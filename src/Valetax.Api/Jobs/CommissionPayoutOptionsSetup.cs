using Valetax.Infrastructure.Options;

namespace Valetax.Api.Jobs;

public sealed class CommissionPayoutOptionsSetup : IValetaxOptionsSetup
{
    public void OnConfigure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CommissionPayoutOptions>(configuration.GetSection(CommissionPayoutOptions.SectionName));
    }
}