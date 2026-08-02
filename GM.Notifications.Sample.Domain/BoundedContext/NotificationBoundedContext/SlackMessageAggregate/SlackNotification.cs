using GM.EntityFramework.Domain.Abstractions;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate;

public class SlackNotification : GM.Notifications.Domain.Entities.SlackNotification, IAggregateRoot
{
    private SlackNotification()
    {
    }

    private SlackNotification(
        string channel,
        string message,
        int maxRetries,
        DateTime? scheduledAtUtc,
        string? correlationId)
        : base(channel, message, maxRetries, scheduledAtUtc, correlationId)
    {
    }

    public static SlackNotification Create(
        string channel,
        string message,
        int maxRetries,
        DateTime? scheduledAtUtc = null,
        string? correlationId = null) =>
        new(channel, message, maxRetries, scheduledAtUtc, correlationId);
}
