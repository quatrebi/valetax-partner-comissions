using Quartz;
using Valetax.Infrastructure.Jobs;

namespace Valetax.Api.Jobs;

public sealed class PayoutCommissionsJobSetup : IValetaxJobSetup
{
    public void OnConfigure(IQuartzBuilder builder, IConfiguration configuration)
    {
        var options = configuration.GetSection(CommissionPayoutOptions.SectionName).Get<CommissionPayoutOptions>()
                      ?? new CommissionPayoutOptions();

        var jobKey = new JobKey("payout-commissions");
        builder.AddJob<PayoutCommissionsJob>(opts => opts.WithIdentity(jobKey));
        builder.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("payout-commissions-trigger")
            .StartNow()
            .WithSimpleSchedule(schedule => schedule
                .WithInterval(options.PollingInterval)
                .RepeatForever()));
    }
}