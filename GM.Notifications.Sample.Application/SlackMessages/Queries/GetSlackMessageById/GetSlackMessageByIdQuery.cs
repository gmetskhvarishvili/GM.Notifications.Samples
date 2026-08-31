using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.SlackMessages.Queries.GetSlackMessageById;

public sealed record GetSlackMessageByIdQuery(Guid Id) : IRequest<SlackNotificationDto?>;

public sealed record SlackNotificationDto(
    Guid Id,
    string Channel,
    string Message,
    NotificationStatus Status,
    int RetryCount,
    int MaxRetries,
    DateTime CreatedAtUtc,
    DateTime? ScheduledAtUtc,
    DateTime? SentAtUtc,
    DateTime? FailedAtUtc,
    string? FailureReason,
    string? CorrelationId);

public sealed class GetSlackMessageByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSlackMessageByIdQuery, SlackNotificationDto?>
{
    public async Task<SlackNotificationDto?> Handle(GetSlackMessageByIdQuery request, CancellationToken cancellationToken)
    {
        var n = await unitOfWork.SlackNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (n is null) return null;

        return new SlackNotificationDto(n.Id, n.Channel, n.Message,
            n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
            n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId);
    }
}
