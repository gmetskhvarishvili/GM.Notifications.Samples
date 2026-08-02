using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Smses.Commands.CancelSms;

public sealed record CancelSmsCommand(Guid Id) : IRequest<CancelSmsResult>;

public sealed record CancelSmsResult(bool Success, string? Message = null);

public class CancelSmsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelSmsCommand, CancelSmsResult>
{
    public async Task<CancelSmsResult> Handle(CancelSmsCommand request, CancellationToken cancellationToken)
    {
        var notification = await unitOfWork.SmsNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (notification is null)
            return new CancelSmsResult(false, "SMS notification not found.");

        if (notification.Status != NotificationStatus.Pending)
            return new CancelSmsResult(false, $"Cannot cancel a notification with status '{notification.Status}'.");

        notification.Cancel();
        unitOfWork.SmsNotificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelSmsResult(true);
    }
}
