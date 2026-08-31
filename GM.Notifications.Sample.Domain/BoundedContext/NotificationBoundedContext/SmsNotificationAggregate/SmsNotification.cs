using GM.EntityFramework.Domain.Abstractions;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate;

public sealed class SmsNotification : GM.Notifications.Domain.Entities.SmsNotification, IAggregateRoot
{
    private SmsNotification()
    {
    }

    private SmsNotification(
        string phoneNumber,
        string body,
        int maxRetries,
        DateTime? scheduledAtUtc,
        string? correlationId,
        Guid? userId)
        : base(phoneNumber, body, maxRetries, scheduledAtUtc, correlationId, userId)
    {
    }

    public static SmsNotification Create(
        string phoneNumber,
        string body,
        int maxRetries,
        DateTime? scheduledAtUtc = null,
        string? correlationId = null,
        Guid? userId = null) =>
        new(phoneNumber, body, maxRetries, scheduledAtUtc, correlationId, userId);
}
