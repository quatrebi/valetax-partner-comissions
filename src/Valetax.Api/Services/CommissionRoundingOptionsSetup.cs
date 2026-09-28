using Valetax.Infrastructure.Options;

namespace Valetax.Api.Services;

public sealed class CommissionRoundingOptionsSetup : IValetaxOptionsSetup
{
    public void OnConfigure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CommissionRoundingOptions>(configuration.GetSection(CommissionRoundingOptions.SectionName));
    }
}