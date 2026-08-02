using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Smses.Commands.SendSms;

public sealed record SendSmsCommand(
    string PhoneNumber,
    string Body,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null,
    Guid? UserId = null) : IRequest<SendSmsResult>;

public sealed record SendSmsResult(Guid NotificationId);

public class SendSmsCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendSmsCommand, SendSmsResult>
{
    public async Task<SendSmsResult> Handle(SendSmsCommand request, CancellationToken cancellationToken)
    {
        var notification = SmsNotification.Create(
            request.PhoneNumber,
            request.Body,
            options.MaxRetries,
            request.ScheduledAtUtc,
            request.CorrelationId,
            request.UserId);

        await unitOfWork.SmsNotificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SendSmsResult(notification.Id);
    }
}
