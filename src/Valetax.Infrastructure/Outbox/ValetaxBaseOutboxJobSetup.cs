using Microsoft.Extensions.Configuration;
using Quartz;
using Valetax.Infrastructure.Jobs;

namespace Valetax.Infrastructure.Outbox;

public abstract class ValetaxBaseOutboxJobSetup<TJob> : IValetaxJobSetup
    where TJob : ValetaxBaseOutboxJob
{
    public void OnConfigure(IQuartzBuilder builder, IConfiguration configuration)
    {
        var options = configuration.GetSection(OutboxOptions.SectionName).Get<OutboxOptions>()
                      ?? new OutboxOptions();

        var jobKey = new JobKey("publish-outbox-messages");
        builder.AddJob<TJob>(opts => opts.WithIdentity(jobKey));
        builder.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("publish-outbox-messages-trigger")
            .StartNow()
            .WithSimpleSchedule(schedule => schedule
                .WithInterval(options.PollingInterval)
                .RepeatForever()));
    }
}