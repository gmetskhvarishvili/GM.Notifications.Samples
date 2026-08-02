using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Emails.Queries.GetEmailById;

public sealed record GetEmailByIdQuery(Guid Id) : IRequest<EmailNotificationDto?>;

public sealed record EmailNotificationDto(
    Guid Id,
    string To,
    string Subject,
    string Body,
    bool IsHtml,
    NotificationStatus Status,
    int RetryCount,
    int MaxRetries,
    DateTime CreatedAtUtc,
    DateTime? ScheduledAtUtc,
    DateTime? SentAtUtc,
    DateTime? FailedAtUtc,
    string? FailureReason,
    string? CorrelationId);

public class GetEmailByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetEmailByIdQuery, EmailNotificationDto?>
{
    public async Task<EmailNotificationDto?> Handle(GetEmailByIdQuery request, CancellationToken cancellationToken)
    {
        var n = await unitOfWork.EmailNotificationRepository.FirstOrDefaultAsync(
            x => x.Id == request.Id, false, null, cancellationToken);

        if (n is null) return null;

        return new EmailNotificationDto(n.Id, n.To, n.Subject, n.Body, n.IsHtml,
            n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
            n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId);
    }
}
