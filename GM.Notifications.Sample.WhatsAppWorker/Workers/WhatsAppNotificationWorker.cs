using GM.Notifications.Domain.Enums;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;
using GM.Notifications.WhatsApp.Abstractions;
using GM.Notifications.WhatsApp.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GM.Notifications.Sample.WhatsAppWorker.Workers;

public sealed class WhatsAppNotificationWorker(
    ILogger<WhatsAppNotificationWorker> logger,
    IServiceScopeFactory scopeFactory,
    NotificationOptions options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("WhatsAppNotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessBatchAsync(stoppingToken); }
            catch (Exception ex) when (ex is not OperationCanceledException)
            { logger.LogError(ex, "Unhandled error in WhatsAppNotificationWorker."); }

            await Task.Delay(TimeSpan.FromSeconds(options.WorkerIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var sender = scope.ServiceProvider.GetRequiredService<IWhatsAppSenderService>();
        var nowUtc = DateTime.UtcNow;

        var pending = await unitOfWork.WhatsAppNotificationRepository.FindAsync(
            x => x.Status == NotificationStatus.Pending
                 && x.RetryCount < x.MaxRetries
                 && (x.ScheduledAtUtc == null || x.ScheduledAtUtc <= nowUtc),
            false, null, ct);

        var batch = pending.Take(options.WorkerBatchSize).ToList();
        if (batch.Count == 0) return;

        logger.LogInformation("Processing {Count} pending WhatsApp notifications.", batch.Count);

        foreach (var notification in batch)
        {
            try
            {
                var message = new WhatsAppMessageDto(
                    notification.Id,
                    notification.PhoneNumber,
                    notification.Body,
                    notification.CorrelationId);

                await sender.SendAsync(message, ct);
                notification.MarkSent(DateTime.UtcNow);
                logger.LogInformation("WhatsApp notification {Id} sent.", notification.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send WhatsApp notification {Id}.", notification.Id);
                notification.MarkFailed(ex.Message, DateTime.UtcNow);
            }

            unitOfWork.WhatsAppNotificationRepository.Update(notification);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
