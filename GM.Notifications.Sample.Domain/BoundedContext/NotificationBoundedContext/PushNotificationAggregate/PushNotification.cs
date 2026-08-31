using GM.EntityFramework.Domain.Abstractions;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate;

public sealed class PushNotification : GM.Notifications.Domain.Entities.PushNotification, IAggregateRoot
{
    private PushNotification()
    {
    }

    private PushNotification(
        string deviceToken,
        string title,
        string body,
        int maxRetries,
        string? data,
        DateTime? scheduledAtUtc,
        string? correlationId)
        : base(deviceToken, title, body, maxRetries, data, scheduledAtUtc, correlationId)
    {
    }

    public static PushNotification Create(
        string deviceToken,
        string title,
        string body,
        int maxRetries,
        string? data = null,
        DateTime? scheduledAtUtc = null,
        string? correlationId = null) =>
        new(deviceToken, title, body, maxRetries, data, scheduledAtUtc, correlationId);
}
