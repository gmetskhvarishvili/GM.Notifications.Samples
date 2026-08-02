using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Pushes.Commands.SendPushesBatch;

public sealed record SendPushesBatchCommand(
    IReadOnlyList<SendPushBatchItem> Items) : IRequest<SendPushesBatchResult>;

public sealed record SendPushBatchItem(
    string DeviceToken,
    string Title,
    string Body,
    string? Data = null,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);

public sealed record SendPushesBatchResult(IReadOnlyList<Guid> NotificationIds);

public class SendPushesBatchCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendPushesBatchCommand, SendPushesBatchResult>
{
    public async Task<SendPushesBatchResult> Handle(SendPushesBatchCommand request, CancellationToken cancellationToken)
    {
        var ids = new List<Guid>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var notification = PushNotification.Create(
                item.DeviceToken, item.Title, item.Body,
                options.MaxRetries, item.Data, item.ScheduledAtUtc, item.CorrelationId);

            await unitOfWork.PushNotificationRepository.AddAsync(notification, cancellationToken);
            ids.Add(notification.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SendPushesBatchResult(ids);
    }
}
