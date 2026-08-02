using GM.EntityFramework.Persistence.Repositories;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Persistence.Context;

namespace GM.Notifications.Sample.Persistence.Repositories;

public class PushNotificationRepository(ApplicationDbContext context)
    : GenericRepository<PushNotification, ApplicationDbContext>(context), IPushNotificationRepository;
