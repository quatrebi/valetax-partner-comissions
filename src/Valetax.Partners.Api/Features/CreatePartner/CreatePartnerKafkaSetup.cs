using MassTransit;
using Valetax.Infrastructure.Kafka;
using Valetax.Partners.Api.Contracts.IntegrationEvents;

namespace Valetax.Partners.Api.Features.CreatePartner;

public sealed class CreatePartnerKafkaSetup : IValetaxKafkaSetup
{
    public void OnConfigureRider(IRiderRegistrationConfigurator rider)
    {
        rider.AddProducer<string, PartnerCreatedIntegrationEvent>(
            PartnerCreatedIntegrationEvent.Topic,
            context => context.Message.PartnerExternalId.ToString(),
            (_, producer) =>
            {
                producer.EnableIdempotence = true;
                producer.MessageTimeout = TimeSpan.FromSeconds(10);
            });
    }
}