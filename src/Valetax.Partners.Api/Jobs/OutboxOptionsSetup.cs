using Valetax.Infrastructure.Options;
using Valetax.Infrastructure.Outbox;

namespace Valetax.Partners.Api.Jobs;

public sealed class OutboxOptionsSetup : IValetaxOptionsSetup
{
    public void OnConfigure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
    }
}