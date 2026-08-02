using GM.Notifications.Domain.Enums;
using GM.Notifications.Email.Abstractions;
using GM.Notifications.Email.Abstractions.Dtos;
using GM.Notifications.Options;
using GM.Notifications.Sample.Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GM.Notifications.Sample.EmailWorker.Workers;

public class EmailNotificationWorker(
    ILogger<EmailNotificationWorker> logger,
    IServiceScopeFactory scopeFactory,
    NotificationOptions options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("EmailNotificationWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessBatchAsync(stoppingToken); }
            catch (Exception ex) when (ex is not OperationCanceledException)
            { logger.LogError(ex, "Unhandled error in EmailNotificationWorker."); }

            await Task.Delay(TimeSpan.FromSeconds(options.WorkerIntervalSeconds), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var sender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
        var nowUtc = DateTime.UtcNow;

        var pending = await unitOfWork.EmailNotificationRepository.FindAsync(
            x => x.Status == NotificationStatus.Pending
                 && x.RetryCount < x.MaxRetries
                 && (x.ScheduledAtUtc == null || x.ScheduledAtUtc <= nowUtc),
            false, null, ct);

        var batch = pending.Take(options.WorkerBatchSize).ToList();
        if (batch.Count == 0) return;

        logger.LogInformation("Processing {Count} pending email notifications.", batch.Count);

        foreach (var notification in batch)
        {
            try
            {
                // Map domain entity to DTO expected by the sender service
                var message = new EmailMessageDto(
                    notification.Id,
                    notification.To,
                    notification.Subject,
                    notification.Body,
                    notification.IsHtml,
                    notification.CorrelationId);

                await sender.SendAsync(message, ct);
                notification.MarkSent(DateTime.UtcNow);
                logger.LogInformation("Email notification {Id} sent.", notification.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email notification {Id}.", notification.Id);
                notification.MarkFailed(ex.Message, DateTime.UtcNow);
            }

            unitOfWork.EmailNotificationRepository.Update(notification);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
