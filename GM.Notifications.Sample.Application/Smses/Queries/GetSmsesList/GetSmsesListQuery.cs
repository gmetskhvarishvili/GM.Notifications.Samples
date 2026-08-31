using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.Smses.Queries.GetSmsById;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Smses.Queries.GetSmsesList;

public sealed record GetSmsesListQuery(
    NotificationStatus? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<GetSmsesListResult>;

public sealed record GetSmsesListResult(IReadOnlyList<SmsNotificationDto> Items, int TotalCount);

public sealed class GetSmsesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSmsesListQuery, GetSmsesListResult>
{
    public async Task<GetSmsesListResult> Handle(GetSmsesListQuery request, CancellationToken cancellationToken)
    {
        var all = await unitOfWork.SmsNotificationRepository.FindAsync(
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
            .Select(n => new SmsNotificationDto(n.Id, n.PhoneNumber, n.Body,
                n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
                n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId))
            .ToList();

        return new GetSmsesListResult(items, total);
    }
}
