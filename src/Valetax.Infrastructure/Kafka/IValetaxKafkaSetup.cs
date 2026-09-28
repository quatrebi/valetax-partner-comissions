using MassTransit;

namespace Valetax.Infrastructure.Kafka;

public interface IValetaxKafkaSetup
{
    void OnConfigureRider(IRiderRegistrationConfigurator rider);

    void OnConfigureKafka(IRiderRegistrationContext context, IKafkaFactoryConfigurator kafka)
    {
    }
}