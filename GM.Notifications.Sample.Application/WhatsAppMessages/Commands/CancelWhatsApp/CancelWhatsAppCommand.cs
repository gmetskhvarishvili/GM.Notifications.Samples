using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.WhatsAppMessages.Commands.CancelWhatsApp;

public sealed record CancelWhatsAppCommand(Guid Id) : IRequest<CancelWhatsAppResult>;

public sealed record CancelWhatsAppResult(bool Success, string? Message = null);

public class CancelWhatsAppCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelWhatsAppCommand, CancelWhatsAppResult>
{
    public async Task<CancelWhatsAppResult> Handle(CancelWhatsAppCommand request, CancellationToken cancellationToken)
    {
        var notification = await unitOfWork.WhatsAppNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (notification is null)
            return new CancelWhatsAppResult(false, "WhatsApp notification not found.");

        if (notification.Status != NotificationStatus.Pending)
            return new CancelWhatsAppResult(false, $"Cannot cancel a notification with status '{notification.Status}'.");

        notification.Cancel();
        unitOfWork.WhatsAppNotificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelWhatsAppResult(true);
    }
}
