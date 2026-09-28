using Microsoft.Extensions.Configuration;
using Quartz;

namespace Valetax.Infrastructure.Jobs;

public interface IValetaxJobSetup
{
    void OnConfigure(IQuartzBuilder builder, IConfiguration configuration);
}