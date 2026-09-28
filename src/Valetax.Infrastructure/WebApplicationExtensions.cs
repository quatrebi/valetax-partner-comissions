using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Valetax.Infrastructure.Endpoints;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Infrastructure;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication UseValetax<TProgram>()
        {
            app.UseExceptionHandler();
            app.UseStatusCodePages();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.MapHealthChecks("/health");
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });

            UseValetaxMinimalApi<TProgram>(app);

            return app;
        }

        public async Task SyncDbMigrationsAsync<TDbContext>()
            where TDbContext : DbContext, IDbContext
        {
            await using var scope = app.Services.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<TDbContext>().Database.MigrateAsync();
        }
    }

    private static void UseValetaxMinimalApi<TProgram>(WebApplication app)
    {
        var endpointTypes = typeof(TProgram).Assembly.GetTypes()
            .Where(t => typeof(IApiEndpoint).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

        foreach (var endpointType in endpointTypes)
        {
            var apiEndpoint = (IApiEndpoint)Activator.CreateInstance(endpointType)!;
            var requestType = GetRequestType(endpointType);

            if (requestType is null)
            {
                apiEndpoint.UseMetadata(app);
                continue;
            }

            var filter = (IEndpointFilter)Activator.CreateInstance(
                typeof(ValidationEndpointFilter<>).MakeGenericType(requestType))!;

            var group = app.MapGroup(string.Empty);
            group.AddEndpointFilter(filter);
            apiEndpoint.UseMetadata(group);
        }
    }

    private static Type? GetRequestType(Type endpointType) => endpointType.GetInterfaces()
        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IApiEndpoint<>))
        ?.GetGenericArguments()[0];
}