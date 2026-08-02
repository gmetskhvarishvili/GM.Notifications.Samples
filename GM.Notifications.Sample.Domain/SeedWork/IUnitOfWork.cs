using GM.EntityFramework.Domain.Repositories;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate.Interfaces;

namespace GM.Notifications.Sample.Domain.SeedWork;

public interface IUnitOfWork : IGenericUnitOfWork
{
    IEmailNotificationRepository EmailNotificationRepository { get; }
    ISmsNotificationRepository SmsNotificationRepository { get; }
    IWhatsAppNotificationRepository WhatsAppNotificationRepository { get; }
    IPushNotificationRepository PushNotificationRepository { get; }
    ISlackNotificationRepository SlackNotificationRepository { get; }
    IInboxMessageRepository InboxMessageRepository { get; }
}
