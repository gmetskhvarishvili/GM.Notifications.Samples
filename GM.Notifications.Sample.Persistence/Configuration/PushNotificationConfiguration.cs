using GM.Notifications.Persistence.Configuration;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate;

namespace GM.Notifications.Sample.Persistence.Configuration;

public class PushNotificationConfiguration() : PushNotificationConfiguration<PushNotification>("push", "PushNotifications");
