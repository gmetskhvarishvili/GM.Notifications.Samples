using GM.Notifications.Persistence.Configuration;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate;

namespace GM.Notifications.Sample.Persistence.Configuration;

public sealed class EmailNotificationConfiguration() : EmailNotificationConfiguration<EmailNotification>("email", "EmailNotifications");
