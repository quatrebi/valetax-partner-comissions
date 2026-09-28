using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valetax.Infrastructure.Guards;

namespace Valetax.Infrastructure.Grpc;

public static class GrpcClientExtensions
{
    private const string SectionName = "GrpcServices";

    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder UseGrpcClient<TClient>(string key)
            where TClient : class
        {
            var addressKey = $"{SectionName}:{key}";
            var address = builder.Configuration[addressKey];
            ThrowIfInvalidConfigurationGuard.ThrowIfInvalidConfiguration(address, addressKey);

            builder.Services
                .AddGrpcClient<TClient>(options => options.Address = new Uri(address))
                .AddStandardResilienceHandler(options =>
                {
                    options.CircuitBreaker.FailureRatio = 0.5;
                    options.CircuitBreaker.MinimumThroughput = 10;
                    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
                });

            return builder;
        }
    }
}