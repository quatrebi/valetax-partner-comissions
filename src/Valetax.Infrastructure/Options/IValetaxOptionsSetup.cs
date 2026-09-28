using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Valetax.Infrastructure.Options;

public interface IValetaxOptionsSetup
{
    void OnConfigure(IServiceCollection services, IConfiguration configuration);
}