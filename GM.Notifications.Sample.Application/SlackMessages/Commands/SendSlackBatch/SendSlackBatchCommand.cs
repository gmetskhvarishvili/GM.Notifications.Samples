using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.SlackMessages.Commands.SendSlackBatch;

public sealed record SendSlackBatchCommand(
    IReadOnlyList<SendSlackBatchItem> Items) : IRequest<SendSlackBatchResult>;

public sealed record SendSlackBatchItem(
    string Channel,
    string Message,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);

public sealed record SendSlackBatchResult(IReadOnlyList<Guid> NotificationIds);

public sealed class SendSlackBatchCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendSlackBatchCommand, SendSlackBatchResult>
{
    public async Task<SendSlackBatchResult> Handle(SendSlackBatchCommand request, CancellationToken cancellationToken)
    {
        var ids = new List<Guid>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var notification = SlackNotification.Create(
                item.Channel, item.Message,
                options.MaxRetries, item.ScheduledAtUtc, item.CorrelationId);

            await unitOfWork.SlackNotificationRepository.AddAsync(notification, cancellationToken);
            ids.Add(notification.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SendSlackBatchResult(ids);
    }
}
