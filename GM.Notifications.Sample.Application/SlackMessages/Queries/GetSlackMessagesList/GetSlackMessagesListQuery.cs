using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.SlackMessages.Queries.GetSlackMessageById;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.SlackMessages.Queries.GetSlackMessagesList;

public sealed record GetSlackMessagesListQuery(
    NotificationStatus? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<GetSlackMessagesListResult>;

public sealed record GetSlackMessagesListResult(IReadOnlyList<SlackNotificationDto> Items, int TotalCount);

public class GetSlackMessagesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSlackMessagesListQuery, GetSlackMessagesListResult>
{
    public async Task<GetSlackMessagesListResult> Handle(GetSlackMessagesListQuery request, CancellationToken cancellationToken)
    {
        var all = await unitOfWork.SlackNotificationRepository.FindAsync(
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
            .Select(n => new SlackNotificationDto(n.Id, n.Channel, n.Message,
                n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
                n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId))
            .ToList();

        return new GetSlackMessagesListResult(items, total);
    }
}
