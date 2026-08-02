using GM.EntityFramework.Domain.Exceptions;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.SeedWork;
using GM.Notifications.Sample.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GM.Notifications.Sample.Persistence.UnitOfWork;

public class UnitOfWork(
    ApplicationDbContext context,
    IEmailNotificationRepository emailNotificationRepository,
    ISmsNotificationRepository smsNotificationRepository,
    IWhatsAppNotificationRepository whatsAppNotificationRepository,
    IPushNotificationRepository pushNotificationRepository,
    ISlackNotificationRepository slackNotificationRepository,
    IInboxMessageRepository inboxMessageRepository)
    : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public IEmailNotificationRepository EmailNotificationRepository { get; } = emailNotificationRepository;
    public ISmsNotificationRepository SmsNotificationRepository { get; } = smsNotificationRepository;
    public IWhatsAppNotificationRepository WhatsAppNotificationRepository { get; } = whatsAppNotificationRepository;
    public IPushNotificationRepository PushNotificationRepository { get; } = pushNotificationRepository;
    public ISlackNotificationRepository SlackNotificationRepository { get; } = slackNotificationRepository;
    public IInboxMessageRepository InboxMessageRepository { get; } = inboxMessageRepository;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("A concurrency error occurred.", ex);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction ??= await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await operation();
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }
}
