using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Shared.Infrastructure.Outbox;

public static class OutboxJobConfiguration
{
    public static IServiceCollectionQuartzConfigurator AddOutboxPublisherJob(
        this IServiceCollectionQuartzConfigurator configurator,
        int intervalSeconds = 30)
    {
        var jobKey = new JobKey("OutboxPublisherJob");

        configurator.AddJob<OutboxPublisherJob>(opts => opts.WithIdentity(jobKey));

        configurator.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("OutboxPublisherJob-trigger")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(intervalSeconds)
                .RepeatForever()));

        return configurator;
    }
}

