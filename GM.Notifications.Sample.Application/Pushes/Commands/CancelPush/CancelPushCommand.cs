using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Pushes.Commands.CancelPush;

public sealed record CancelPushCommand(Guid Id) : IRequest<CancelPushResult>;

public sealed record CancelPushResult(bool Success, string? Message = null);

public class CancelPushCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelPushCommand, CancelPushResult>
{
    public async Task<CancelPushResult> Handle(CancelPushCommand request, CancellationToken cancellationToken)
    {
        var notification = await unitOfWork.PushNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (notification is null)
            return new CancelPushResult(false, "Push notification not found.");

        if (notification.Status != NotificationStatus.Pending)
            return new CancelPushResult(false, $"Cannot cancel a notification with status '{notification.Status}'.");

        notification.Cancel();
        unitOfWork.PushNotificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelPushResult(true);
    }
}
