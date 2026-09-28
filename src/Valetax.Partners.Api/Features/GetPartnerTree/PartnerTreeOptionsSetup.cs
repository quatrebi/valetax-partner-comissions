using Valetax.Infrastructure.Options;

namespace Valetax.Partners.Api.Features.GetPartnerTree;

public sealed class PartnerTreeOptionsSetup : IValetaxOptionsSetup
{
    public void OnConfigure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PartnerTreeOptions>(configuration.GetSection(PartnerTreeOptions.SectionName));
    }
}