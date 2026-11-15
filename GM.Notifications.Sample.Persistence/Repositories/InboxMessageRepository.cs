using GM.EntityFramework.Persistence.Repositories;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.Notifications.Sample.Persistence.Context;

namespace GM.Notifications.Sample.Persistence.Repositories;

public sealed class InboxMessageRepository(ApplicationDbContext context)
    : GenericRepository<InboxMessage, ApplicationDbContext>(context), IInboxMessageRepository;
