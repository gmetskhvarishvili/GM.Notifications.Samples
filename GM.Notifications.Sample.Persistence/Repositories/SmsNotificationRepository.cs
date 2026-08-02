using GM.EntityFramework.Persistence.Repositories;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Persistence.Context;

namespace GM.Notifications.Sample.Persistence.Repositories;

public class SmsNotificationRepository(ApplicationDbContext context)
    : GenericRepository<SmsNotification, ApplicationDbContext>(context), ISmsNotificationRepository;
