using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Smses.Queries.GetSmsById;

public sealed record GetSmsByIdQuery(Guid Id) : IRequest<SmsNotificationDto?>;

public sealed record SmsNotificationDto(
    Guid Id,
    string PhoneNumber,
    string Body,
    NotificationStatus Status,
    int RetryCount,
    int MaxRetries,
    DateTime CreatedAtUtc,
    DateTime? ScheduledAtUtc,
    DateTime? SentAtUtc,
    DateTime? FailedAtUtc,
    string? FailureReason,
    string? CorrelationId);

public class GetSmsByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSmsByIdQuery, SmsNotificationDto?>
{
    public async Task<SmsNotificationDto?> Handle(GetSmsByIdQuery request, CancellationToken cancellationToken)
    {
        var n = await unitOfWork.SmsNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (n is null) return null;

        return new SmsNotificationDto(n.Id, n.PhoneNumber, n.Body,
            n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
            n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId);
    }
}
