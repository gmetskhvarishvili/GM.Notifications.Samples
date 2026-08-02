using GM.Notifications.Persistence.Configuration;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate;

namespace GM.Notifications.Sample.Persistence.Configuration;

public class WhatsAppNotificationConfiguration() : WhatsAppNotificationConfiguration<WhatsAppNotification>("whatsapp", "WhatsAppNotifications");
