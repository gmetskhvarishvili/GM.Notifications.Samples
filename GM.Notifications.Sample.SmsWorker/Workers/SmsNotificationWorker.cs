using GM.Notifications.Domain.Enums;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;
using GM.Notifications.SMS.Abstractions;
using GM.Notifications.SMS.Abstractions.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GM.Notifications.Sample.SmsWorker.Workers;

public sealed class SmsNotificationWorker(
    ILogger<SmsNotificationWorker> logger,
    IServiceScopeFactory scopeFactory,
    NotificationOptions options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SmsNotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessBatchAsync(stoppingToken); }
            catch (Exception ex) when (ex is not OperationCanceledException)
            { logger.LogError(ex, "Unhandled error in SmsNotificationWorker."); }

            await Task.Delay(TimeSpan.FromSeconds(options.WorkerIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var sender = scope.ServiceProvider.GetRequiredService<ISmsSenderService>();
        var nowUtc = DateTime.UtcNow;

        var pending = await unitOfWork.SmsNotificationRepository.FindAsync(
            x => x.Status == NotificationStatus.Pending
                 && x.RetryCount < x.MaxRetries
                 && (x.ScheduledAtUtc == null || x.ScheduledAtUtc <= nowUtc),
            false, null, ct);

        var batch = pending.Take(options.WorkerBatchSize).ToList();
        if (batch.Count == 0) return;

        logger.LogInformation("Processing {Count} pending SMS notifications.", batch.Count);

        foreach (var notification in batch)
        {
            try
            {
                var message = new SmsMessageDto(
                    notification.Id,
                    notification.PhoneNumber,
                    notification.Body,
                    notification.CorrelationId);

                await sender.SendAsync(message, ct);
                notification.MarkSent(DateTime.UtcNow);
                logger.LogInformation("SMS notification {Id} sent.", notification.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send SMS notification {Id}.", notification.Id);
                notification.MarkFailed(ex.Message, DateTime.UtcNow);
            }

            unitOfWork.SmsNotificationRepository.Update(notification);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
