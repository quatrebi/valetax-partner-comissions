using MassTransit;
using Microsoft.Extensions.Options;
using Valetax.Infrastructure.Outbox;
using Valetax.Partners.Api.Contracts.IntegrationEvents;
using Valetax.Partners.Api.Persistence;

namespace Valetax.Partners.Api.Jobs;

public sealed class PublishOutboxMessagesJob(
    IPartnersDbContext dbContext,
    ITopicProducer<PartnerCreatedIntegrationEvent> partnerCreatedProducer,
    IOptions<OutboxOptions> options,
    ILogger<PublishOutboxMessagesJob> logger) : ValetaxBaseOutboxJob(dbContext, options, logger)
{
    protected override Task PublishAsync(OutboxMessage message, CancellationToken ct) => message.Topic switch
    {
        PartnerCreatedIntegrationEvent.Topic => partnerCreatedProducer.Produce(
            message.GetPayload<PartnerCreatedIntegrationEvent>(), ct),
        _ => throw new UnsupportedOutboxTopicException(message.Topic)
    };
}