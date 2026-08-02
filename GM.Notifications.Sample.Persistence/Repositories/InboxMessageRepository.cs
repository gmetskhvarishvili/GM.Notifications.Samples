using GM.EntityFramework.Persistence.Repositories;
using GM.Messaging.Persistence.Inbox;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.Notifications.Sample.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace GM.Notifications.Sample.Persistence.Repositories;

public class InboxMessageRepository(ApplicationDbContext context)
    : GenericRepository<InboxMessage, ApplicationDbContext>(context), IInboxMessageRepository, IInboxStore<InboxMessage>
{
    Task<bool> IInboxStore<InboxMessage>.ExistsAsync(Guid eventId, string consumerName,
        CancellationToken cancellationToken) =>
        context.Set<InboxMessage>()
            .AnyAsync(m => m.EventId == eventId && m.ConsumerName == consumerName, cancellationToken);

    async Task<InboxMessage> IInboxStore<InboxMessage>.CreateAndAddAsync(
        Guid eventId, string consumerName, string eventType, string payload, Guid? userId,
        CancellationToken cancellationToken)
    {
        var message = InboxMessage.Create(eventId, consumerName, eventType, payload, userId);
        await context.Set<InboxMessage>().AddAsync(message, cancellationToken);
        return message;
    }

    Task IInboxStore<InboxMessage>.SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);
}