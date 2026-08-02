using GM.Mediator.Contracts;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.SlackMessages.Commands.SendSlack;

public sealed record SendSlackCommand(
    string Channel,
    string Message,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null) : IRequest<SendSlackResult>;

public sealed record SendSlackResult(Guid NotificationId);

public class SendSlackCommandHandler(IUnitOfWork unitOfWork, NotificationOptions options)
    : IRequestHandler<SendSlackCommand, SendSlackResult>
{
    public async Task<SendSlackResult> Handle(SendSlackCommand request, CancellationToken cancellationToken)
    {
        var notification = SlackNotification.Create(
            request.Channel,
            request.Message,
            options.MaxRetries,
            request.ScheduledAtUtc,
            request.CorrelationId);

        await unitOfWork.SlackNotificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SendSlackResult(notification.Id);
    }
}
