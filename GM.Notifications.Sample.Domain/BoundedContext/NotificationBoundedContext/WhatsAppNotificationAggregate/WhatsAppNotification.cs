using GM.EntityFramework.Domain.Abstractions;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate;

public sealed class WhatsAppNotification : GM.Notifications.Domain.Entities.WhatsAppNotification, IAggregateRoot
{
    private WhatsAppNotification()
    {
    }

    private WhatsAppNotification(
        string phoneNumber,
        string body,
        int maxRetries,
        DateTime? scheduledAtUtc,
        string? correlationId,
        Guid? userId)
        : base(phoneNumber, body, maxRetries, scheduledAtUtc, correlationId, userId)
    {
    }

    public static WhatsAppNotification Create(
        string phoneNumber,
        string body,
        int maxRetries,
        DateTime? scheduledAtUtc = null,
        string? correlationId = null,
        Guid? userId = null) =>
        new(phoneNumber, body, maxRetries, scheduledAtUtc, correlationId, userId);
}
