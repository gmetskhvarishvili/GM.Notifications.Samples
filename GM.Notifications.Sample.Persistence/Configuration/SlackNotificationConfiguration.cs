using GM.Notifications.Persistence.Configuration;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate;

namespace GM.Notifications.Sample.Persistence.Configuration;

public sealed class SlackNotificationConfiguration() : SlackNotificationConfiguration<SlackNotification>("slack", "SlackNotifications");
