using GM.Notifications.Persistence.Configuration;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate;

namespace GM.Notifications.Sample.Persistence.Configuration;

public class SmsNotificationConfiguration() : SmsNotificationConfiguration<SmsNotification>("sms", "SmsNotifications");
