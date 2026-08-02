using GM.EntityFramework.Domain.Repositories;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate.Interfaces;

public interface ISlackNotificationRepository : IGenericRepository<SlackNotification>;
