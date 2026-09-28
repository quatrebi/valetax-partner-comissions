using System.Data.Common;
using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Valetax.Infrastructure.Kafka;
using Valetax.Partners.Api.Contracts.IntegrationEvents;

namespace Valetax.Wallets.Api.Features.CreateWallet;

public sealed class CreateWalletKafkaSetup : IValetaxKafkaSetup
{
    private const string ConsumerGroup = "wallets-api";
    private const int TopicPartitions = 6;

    public void OnConfigureRider(IRiderRegistrationConfigurator rider)
    {
        rider.AddConsumer<PartnerCreatedConsumer>();
    }

    public void OnConfigureKafka(IRiderRegistrationContext context, IKafkaFactoryConfigurator kafka)
    {
        kafka.TopicEndpoint<PartnerCreatedIntegrationEvent>(PartnerCreatedIntegrationEvent.Topic, ConsumerGroup, e =>
        {
            e.AutoOffsetReset = AutoOffsetReset.Earliest;
            e.CreateIfMissing(topic => topic.NumPartitions = TopicPartitions);
            e.UseMessageRetry(retry =>
            {
                retry.Handle<DbException>();
                retry.Handle<DbUpdateException>();
                retry.Handle<TimeoutException>();
                retry.Exponential(int.MaxValue, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(5));
            });
            e.ConfigureConsumer<PartnerCreatedConsumer>(context);
        });
    }
}