using GM.EntityFramework.Persistence.Repositories;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Persistence.Context;

namespace GM.Notifications.Sample.Persistence.Repositories;

public sealed class WhatsAppNotificationRepository(ApplicationDbContext context)
    : GenericRepository<WhatsAppNotification, ApplicationDbContext>(context), IWhatsAppNotificationRepository;
