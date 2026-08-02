using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.WhatsAppMessages.Queries.GetWhatsAppById;

public sealed record GetWhatsAppByIdQuery(Guid Id) : IRequest<WhatsAppNotificationDto?>;

public sealed record WhatsAppNotificationDto(
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

public class GetWhatsAppByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWhatsAppByIdQuery, WhatsAppNotificationDto?>
{
    public async Task<WhatsAppNotificationDto?> Handle(GetWhatsAppByIdQuery request, CancellationToken cancellationToken)
    {
        var n = await unitOfWork.WhatsAppNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (n is null) return null;

        return new WhatsAppNotificationDto(n.Id, n.PhoneNumber, n.Body,
            n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
            n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId);
    }
}
