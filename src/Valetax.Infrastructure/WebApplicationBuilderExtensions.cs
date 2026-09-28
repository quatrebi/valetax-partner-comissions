using System.Text.Json.Serialization;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Quartz;
using Valetax.Infrastructure.Exceptions;
using Valetax.Infrastructure.Guards;
using Valetax.Infrastructure.Jobs;
using Valetax.Infrastructure.Kafka;
using Valetax.Infrastructure.Options;
using Valetax.Infrastructure.Pagination;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder UseValetaxInfrastructure<TProgram>()
            where TProgram : class
        {
            builder.Services.AddOpenApi();
            builder.Services.AddGrpc();
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ValetaxExceptionHandler>();
            builder.Services.AddValidatorsFromAssembly(typeof(TProgram).Assembly);
            builder.Services.AddValidatorsFromAssemblyContaining<PageDtoValidator>();
            builder.Services.ConfigureHttpJsonOptions(opts =>
                opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.Configure<HostOptions>(opts =>
                opts.ShutdownTimeout = builder.Configuration.GetValue("ShutdownTimeout", TimeSpan.FromSeconds(30)));

            UseValetaxOtel(builder);
            UseValetaxOptions<TProgram>(builder.Services, builder.Configuration);
            UseValetaxKafka<TProgram>(builder.Services, builder.Configuration);
            UseValetaxQuartz<TProgram>(builder.Services, builder.Configuration);

            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

            return builder;
        }

        public IHostApplicationBuilder UseDbContext<TIDbContext, TDbContext>()
            where TDbContext : DbContext, TIDbContext
            where TIDbContext : IDbContext
        {
            builder.Services.AddDbContext<TDbContext>(opts =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    opts.EnableSensitiveDataLogging();
                    opts.EnableDetailedErrors();
                }

                opts.UseNpgsql(builder.Configuration.GetConnectionString(typeof(TDbContext).Name));
            });

            builder.Services.AddScoped(typeof(TIDbContext), x => x.GetRequiredService<TDbContext>());
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<TDbContext>(name: typeof(TDbContext).Name);

            return builder;
        }
    }

    private static void UseValetaxOtel(IHostApplicationBuilder builder)
    {
        builder.Logging.Configure(options =>
            options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId);

        builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("Npgsql")
                .AddMeter("MassTransit")
                .AddMeter(builder.Environment.ApplicationName))
            .WithTracing(tracing => tracing
                .AddSource(builder.Environment.ApplicationName)
                .AddSource("MassTransit")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddNpgsql());

        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
    }

    private static void UseValetaxOptions<TProgram>(IServiceCollection services, IConfiguration configuration)
        where TProgram : class
    {
        var setups = CreateSetups<TProgram, IValetaxOptionsSetup>();

        setups.ForEach(setup => setup.OnConfigure(services, configuration));
    }

    private static void UseValetaxKafka<TProgram>(IServiceCollection services, IConfiguration configuration)
        where TProgram : class
    {
        var setups = CreateSetups<TProgram, IValetaxKafkaSetup>();

        if (setups.Count == 0)
            return;

        var kafkaHost = configuration.GetSection(KafkaOptions.SectionName).Get<KafkaOptions>()?.Host;
        ThrowIfInvalidConfigurationGuard.ThrowIfInvalidConfiguration(
            kafkaHost,
            $"{KafkaOptions.SectionName}:{nameof(KafkaOptions.Host)}");

        services.AddMassTransit(bus =>
        {
            bus.UsingInMemory();

            bus.AddRider(rider =>
            {
                setups.ForEach(setup => setup.OnConfigureRider(rider));

                rider.UsingKafka((ctx, kafka) =>
                {
                    kafka.Host(kafkaHost);
                    setups.ForEach(setup => setup.OnConfigureKafka(ctx, kafka));
                });
            });
        });
    }

    private static void UseValetaxQuartz<TProgram>(IServiceCollection services, IConfiguration configuration)
        where TProgram : class
    {
        var jobs = CreateSetups<TProgram, IValetaxJobSetup>();

        if (jobs.Count == 0)
            return;

        services.AddQuartz(quartz =>
            jobs.ForEach(job => job.OnConfigure(quartz, configuration)));

        services.AddQuartzHostedService(opts =>
            opts.WaitForJobsToComplete = true);
    }

    private static List<TSetup> CreateSetups<TProgram, TSetup>() =>
    [
        //TODO: need to pass the Array of Assemblies to scan types, from all. but as test-task is okay
        .. typeof(TProgram).Assembly.GetTypes()
            .Where(t => typeof(TSetup).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .Select(t => (TSetup)Activator.CreateInstance(t)!)
    ];
}