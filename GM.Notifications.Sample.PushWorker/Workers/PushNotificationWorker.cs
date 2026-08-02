using GM.Notifications.Domain.Enums;
using GM.Notifications.Options;
using GM.Notifications.Push.Abstractions;
using GM.Notifications.Push.Dtos;
using GM.Notifications.Sample.Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GM.Notifications.Sample.PushWorker.Workers;

public class PushNotificationWorker(
    ILogger<PushNotificationWorker> logger,
    IServiceScopeFactory scopeFactory,
    NotificationOptions options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PushNotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessBatchAsync(stoppingToken); }
            catch (Exception ex) when (ex is not OperationCanceledException)
            { logger.LogError(ex, "Unhandled error in PushNotificationWorker."); }

            await Task.Delay(TimeSpan.FromSeconds(options.WorkerIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var sender = scope.ServiceProvider.GetRequiredService<IPushSenderService>();
        var nowUtc = DateTime.UtcNow;

        var pending = await unitOfWork.PushNotificationRepository.FindAsync(
            x => x.Status == NotificationStatus.Pending
                 && x.RetryCount < x.MaxRetries
                 && (x.ScheduledAtUtc == null || x.ScheduledAtUtc <= nowUtc),
            false, null, ct);

        var batch = pending.Take(options.WorkerBatchSize).ToList();
        if (batch.Count == 0) return;

        logger.LogInformation("Processing {Count} pending push notifications.", batch.Count);

        foreach (var notification in batch)
        {
            try
            {
                var message = new PushMessageDto(
                    notification.Id,
                    notification.DeviceToken,
                    notification.Title,
                    notification.Body,
                    notification.Data,
                    notification.CorrelationId);

                await sender.SendAsync(message, ct);
                notification.MarkSent(DateTime.UtcNow);
                logger.LogInformation("Push notification {Id} sent.", notification.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send push notification {Id}.", notification.Id);
                notification.MarkFailed(ex.Message, DateTime.UtcNow);
            }

            unitOfWork.PushNotificationRepository.Update(notification);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
