using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Emails.Commands.SendEmailsBatch;

public sealed record SendEmailsBatchCommand(
    IReadOnlyList<SendEmailBatchItem> Items) : IRequest<SendEmailsBatchResult>;

public sealed record SendEmailBatchItem(
    string To,
    string Subject,
    string Body,
    bool IsHtml = false,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);

public sealed record SendEmailsBatchResult(IReadOnlyList<Guid> NotificationIds);

public sealed class SendEmailsBatchCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendEmailsBatchCommand, SendEmailsBatchResult>
{
    public async Task<SendEmailsBatchResult> Handle(SendEmailsBatchCommand request, CancellationToken cancellationToken)
    {
        var ids = new List<Guid>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var notification = EmailNotification.Create(
                item.To, item.Subject, item.Body, item.IsHtml,
                options.MaxRetries, item.ScheduledAtUtc, item.CorrelationId);

            await unitOfWork.EmailNotificationRepository.AddAsync(notification, cancellationToken);
            ids.Add(notification.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SendEmailsBatchResult(ids);
    }
}
