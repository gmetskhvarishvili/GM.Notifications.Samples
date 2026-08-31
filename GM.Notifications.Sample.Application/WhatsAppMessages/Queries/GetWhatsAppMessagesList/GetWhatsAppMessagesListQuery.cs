using GM.Mediator.Contracts;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.WhatsAppMessages.Queries.GetWhatsAppById;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Application.WhatsAppMessages.Queries.GetWhatsAppMessagesList;

public sealed record GetWhatsAppMessagesListQuery(
    NotificationStatus? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<GetWhatsAppMessagesListResult>;

public sealed record GetWhatsAppMessagesListResult(IReadOnlyList<WhatsAppNotificationDto> Items, int TotalCount);

public sealed class GetWhatsAppMessagesListQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetWhatsAppMessagesListQuery, GetWhatsAppMessagesListResult>
{
    public async Task<GetWhatsAppMessagesListResult> Handle(GetWhatsAppMessagesListQuery request, CancellationToken cancellationToken)
    {
        var all = await unitOfWork.WhatsAppNotificationRepository.FindAsync(
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
            .Select(n => new WhatsAppNotificationDto(n.Id, n.PhoneNumber, n.Body,
                n.Status, n.RetryCount, n.MaxRetries, n.CreatedAtUtc,
                n.ScheduledAtUtc, n.SentAtUtc, n.FailedAtUtc, n.FailureReason, n.CorrelationId))
            .ToList();

        return new GetWhatsAppMessagesListResult(items, total);
    }
}
