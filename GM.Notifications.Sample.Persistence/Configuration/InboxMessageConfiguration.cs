using GM.Messaging.Persistence.Configuration;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;

namespace GM.Notifications.Sample.Persistence.Configuration;

public sealed class InboxMessageConfiguration() : InboxMessageConfiguration<InboxMessage>("inbox_messages");
