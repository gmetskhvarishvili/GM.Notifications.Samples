using GM.EntityFramework.Domain.Abstractions;

namespace GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate;

public class EmailNotification : GM.Notifications.Domain.Entities.EmailNotification, IAggregateRoot
{
    // For EF materialization; delegates to the protected base parameterless constructor.
    private EmailNotification()
    {
    }

    // Delegates to the base constructor so all invariants run.
    private EmailNotification(
        string to,
        string subject,
        string body,
        bool isHtml,
        int maxRetries,
        DateTime? scheduledAtUtc,
        string? correlationId,
        Guid? userId)
        : base(to, subject, body, isHtml, maxRetries, scheduledAtUtc, correlationId, userId)
    {
    }

    public static EmailNotification Create(
        string to,
        string subject,
        string body,
        bool isHtml,
        int maxRetries,
        DateTime? scheduledAtUtc = null,
        string? correlationId = null,
        Guid? userId = null) =>
        new(to, subject, body, isHtml, maxRetries, scheduledAtUtc, correlationId, userId);
}
