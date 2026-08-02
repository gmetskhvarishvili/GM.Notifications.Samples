using GM.EntityFramework.Domain.Repositories;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate.Interfaces;

public interface IWhatsAppNotificationRepository : IGenericRepository<WhatsAppNotification>;
