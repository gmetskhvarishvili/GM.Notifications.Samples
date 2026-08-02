using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Emails.Commands.CancelEmail;

public sealed record CancelEmailCommand(Guid Id) : IRequest<CancelEmailResult>;

public sealed record CancelEmailResult(bool Success, string? Message = null);

public class CancelEmailCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelEmailCommand, CancelEmailResult>
{
    public async Task<CancelEmailResult> Handle(CancelEmailCommand request, CancellationToken cancellationToken)
    {
        var notification = await unitOfWork.EmailNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (notification is null)
            return new CancelEmailResult(false, "Email notification not found.");

        if (notification.Status != NotificationStatus.Pending)
            return new CancelEmailResult(false, $"Cannot cancel a notification with status '{notification.Status}'.");

        notification.Cancel();
        unitOfWork.EmailNotificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelEmailResult(true);
    }
}
