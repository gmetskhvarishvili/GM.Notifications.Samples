using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Pushes.Commands.SendPush;

public sealed record SendPushCommand(
    string DeviceToken,
    string Title,
    string Body,
    string? Data = null,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null) : IRequest<SendPushResult>;

public sealed record SendPushResult(Guid NotificationId);

public sealed class SendPushCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendPushCommand, SendPushResult>
{
    public async Task<SendPushResult> Handle(SendPushCommand request, CancellationToken cancellationToken)
    {
        var notification = PushNotification.Create(
            request.DeviceToken,
            request.Title,
            request.Body,
            options.MaxRetries,
            request.Data,
            request.ScheduledAtUtc,
            request.CorrelationId);

        await unitOfWork.PushNotificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SendPushResult(notification.Id);
    }
}
