using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.SlackMessages.Commands.CancelSlack;

public sealed record CancelSlackCommand(Guid Id) : IRequest<CancelSlackResult>;

public sealed record CancelSlackResult(bool Success, string? Message = null);

public sealed class CancelSlackCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelSlackCommand, CancelSlackResult>
{
    public async Task<CancelSlackResult> Handle(CancelSlackCommand request, CancellationToken cancellationToken)
    {
        var notification = await unitOfWork.SlackNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (notification is null)
            return new CancelSlackResult(false, "Slack notification not found.");

        if (notification.Status != NotificationStatus.Pending)
            return new CancelSlackResult(false, $"Cannot cancel a notification with status '{notification.Status}'.");

        notification.Cancel();
        unitOfWork.SlackNotificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelSlackResult(true);
    }
}
