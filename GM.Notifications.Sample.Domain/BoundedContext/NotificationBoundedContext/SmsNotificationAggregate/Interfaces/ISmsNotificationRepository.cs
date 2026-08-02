using GM.EntityFramework.Domain.Repositories;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate.Interfaces;

public interface ISmsNotificationRepository : IGenericRepository<SmsNotification>;
