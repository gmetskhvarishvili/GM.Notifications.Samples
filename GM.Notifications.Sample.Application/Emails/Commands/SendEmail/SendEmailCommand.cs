using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Emails.Commands.SendEmail;

public sealed record SendEmailCommand(
    string To,
    string Subject,
    string Body,
    bool IsHtml = false,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null,
    Guid? UserId = null) : IRequest<SendEmailResult>;

public sealed record SendEmailResult(Guid NotificationId);

public sealed class SendEmailCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendEmailCommand, SendEmailResult>
{
    public async Task<SendEmailResult> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var notification = EmailNotification.Create(
            request.To,
            request.Subject,
            request.Body,
            request.IsHtml,
            options.MaxRetries,
            request.ScheduledAtUtc,
            request.CorrelationId,
            request.UserId);

        await unitOfWork.EmailNotificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SendEmailResult(notification.Id);
    }
}
