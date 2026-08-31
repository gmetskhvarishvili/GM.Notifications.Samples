using GM.EntityFramework.Persistence.Repositories;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate.Interfaces;
using GM.Notifications.Sample.Persistence.Context;

namespace GM.Notifications.Sample.Persistence.Repositories;

public sealed class SlackNotificationRepository(ApplicationDbContext context)
    : GenericRepository<SlackNotification, ApplicationDbContext>(context), ISlackNotificationRepository;
