using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.WhatsAppMessages.Commands.SendWhatsApp;

public sealed record SendWhatsAppCommand(
    string PhoneNumber,
    string Body,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null,
    Guid? UserId = null) : IRequest<SendWhatsAppResult>;

public sealed record SendWhatsAppResult(Guid NotificationId);

public sealed class SendWhatsAppCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendWhatsAppCommand, SendWhatsAppResult>
{
    public async Task<SendWhatsAppResult> Handle(SendWhatsAppCommand request, CancellationToken cancellationToken)
    {
        var notification = WhatsAppNotification.Create(
            request.PhoneNumber,
            request.Body,
            options.MaxRetries,
            request.ScheduledAtUtc,
            request.CorrelationId,
            request.UserId);

        await unitOfWork.WhatsAppNotificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SendWhatsAppResult(notification.Id);
    }
}
