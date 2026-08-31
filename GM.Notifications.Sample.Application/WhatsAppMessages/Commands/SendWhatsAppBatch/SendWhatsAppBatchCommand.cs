using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.WhatsAppMessages.Commands.SendWhatsAppBatch;

public sealed record SendWhatsAppBatchCommand(
    IReadOnlyList<SendWhatsAppBatchItem> Items) : IRequest<SendWhatsAppBatchResult>;

public sealed record SendWhatsAppBatchItem(
    string PhoneNumber,
    string Body,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);

public sealed record SendWhatsAppBatchResult(IReadOnlyList<Guid> NotificationIds);

public sealed class SendWhatsAppBatchCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendWhatsAppBatchCommand, SendWhatsAppBatchResult>
{
    public async Task<SendWhatsAppBatchResult> Handle(SendWhatsAppBatchCommand request, CancellationToken cancellationToken)
    {
        var ids = new List<Guid>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var notification = WhatsAppNotification.Create(
                item.PhoneNumber, item.Body,
                options.MaxRetries, item.ScheduledAtUtc, item.CorrelationId);

            await unitOfWork.WhatsAppNotificationRepository.AddAsync(notification, cancellationToken);
            ids.Add(notification.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SendWhatsAppBatchResult(ids);
    }
}
