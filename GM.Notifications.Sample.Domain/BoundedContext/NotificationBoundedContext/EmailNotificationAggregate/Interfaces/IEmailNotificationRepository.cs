using GM.EntityFramework.Domain.Repositories;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate.Interfaces;

public interface IEmailNotificationRepository : IGenericRepository<EmailNotification>;
