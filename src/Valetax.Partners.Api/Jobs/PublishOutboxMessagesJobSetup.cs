using Valetax.Infrastructure.Outbox;

namespace Valetax.Partners.Api.Jobs;

public sealed class PublishOutboxMessagesJobSetup : ValetaxBaseOutboxJobSetup<PublishOutboxMessagesJob>;