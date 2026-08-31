using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.Pushes.Queries.GetPushById;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.Pushes.Queries.GetPushesList;

public sealed record GetPushesListQuery(
    NotificationStatus? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<GetPushesListResult>;

public sealed record GetPushesListResult(IReadOnlyList<PushNotificationDto> Items, int TotalCount);

public sealed class GetPushesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPushesListQuery, GetPushesListResult>
{
    public async Task<GetPushesListResult> Handle(GetPushesListQuery request, CancellationToken cancellationToken)
    {
        var all = await unitOfWork.PushNotificationRepository.FindAsync(
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
            .Select(n => new PushNotificationDto(n.Id, n.DeviceToken, n.Title, n.Body, n.Data,
                n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
                n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId))
            .ToList();

        return new GetPushesListResult(items, total);
    }
}
