using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.Emails.Queries.GetEmailById;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Emails.Queries.GetEmailsList;

public sealed record GetEmailsListQuery(
    NotificationStatus? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<GetEmailsListResult>;

public sealed record GetEmailsListResult(IReadOnlyList<EmailNotificationDto> Items, int TotalCount);

public sealed class GetEmailsListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetEmailsListQuery, GetEmailsListResult>
{
    public async Task<GetEmailsListResult> Handle(GetEmailsListQuery request, CancellationToken cancellationToken)
    {
        var all = await unitOfWork.EmailNotificationRepository.FindAsync(
            x => request.Status == null || x.Status == request.Status,
            false, null, cancellationToken);

        var list = all.ToList();
        var total = list.Count;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

        var items = list
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new EmailNotificationDto(n.Id, n.To, n.Subject, n.Body, n.IsHtml,
                n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
                n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId))
            .ToList();

        return new GetEmailsListResult(items, total);
    }
}
