using GM.EntityFramework.Domain.Repositories;

namespace GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;

// Uses the base GM.Messaging InboxMessage directly (Create/MarkProcessed/MarkFailed live on it).
public interface IInboxMessageRepository : IGenericRepository<InboxMessage>;
