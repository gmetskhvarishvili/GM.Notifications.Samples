using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Pushes.Queries.GetPushById;

public sealed record GetPushByIdQuery(Guid Id) : IRequest<PushNotificationDto?>;

public sealed record PushNotificationDto(
    Guid Id,
    string DeviceToken,
    string Title,
    string Body,
    string? Data,
    NotificationStatus Status,
    int RetryCount,
    int MaxRetries,
    DateTime CreatedAtUtc,
    DateTime? ScheduledAtUtc,
    DateTime? SentAtUtc,
    DateTime? FailedAtUtc,
    string? FailureReason,
    string? CorrelationId);

public class GetPushByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPushByIdQuery, PushNotificationDto?>
{
    public async Task<PushNotificationDto?> Handle(GetPushByIdQuery request, CancellationToken cancellationToken)
    {
        var n = await unitOfWork.PushNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (n is null) return null;

        return new PushNotificationDto(n.Id, n.DeviceToken, n.Title, n.Body, n.Data,
            n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
            n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId);
    }
}
