using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Smses.Commands.SendSmsesBatch;

public sealed record SendSmsesBatchCommand(
    IReadOnlyList<SendSmsBatchItem> Items) : IRequest<SendSmsesBatchResult>;

public sealed record SendSmsBatchItem(
    string PhoneNumber,
    string Body,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);

public sealed record SendSmsesBatchResult(IReadOnlyList<Guid> NotificationIds);

public sealed class SendSmsesBatchCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendSmsesBatchCommand, SendSmsesBatchResult>
{
    public async Task<SendSmsesBatchResult> Handle(SendSmsesBatchCommand request, CancellationToken cancellationToken)
    {
        var ids = new List<Guid>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var notification = SmsNotification.Create(
                item.PhoneNumber, item.Body,
                options.MaxRetries, item.ScheduledAtUtc, item.CorrelationId);

            await unitOfWork.SmsNotificationRepository.AddAsync(notification, cancellationToken);
            ids.Add(notification.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new SendSmsesBatchResult(ids);
    }
}
