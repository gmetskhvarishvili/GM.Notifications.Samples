using System.Text.Json;
using GM.Mediator.Contracts;
using GM.Notifications.Sample.Application.Emails.Commands.SendEmail;
using GM.Notifications.Sample.Application.Smses.Commands.SendSms;
using GM.Notifications.Sample.Application.WhatsAppMessages.Commands.SendWhatsApp;
using GM.Notifications.Sample.Domain.Events.Otp;
using GM.Notifications.Sample.Domain.Events.Users;
using GM.Notifications.Sample.Domain.SeedWork;

namespace GM.Notifications.Sample.Worker.Workers;

public class InboxProcessorWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<InboxProcessorWorker> logger,
    IConfiguration configuration)
    : BackgroundService
{
    private const int DefaultBatchSize = 50;
    private const int DefaultMaxConcurrency = 10;

    private static readonly Dictionary<string, string> EventTypeMap = new()
    {
        { nameof(UserRegisteredIntegrationEvent), typeof(UserRegisteredIntegrationEvent).FullName! },
        { nameof(UserConfirmedIntegrationEvent), typeof(UserConfirmedIntegrationEvent).FullName! },
        { nameof(OtpGeneratedIntegrationEvent), typeof(OtpGeneratedIntegrationEvent).FullName! }
    };

    private readonly TimeSpan _pollInterval = 
        TimeSpan.FromSeconds(configuration.GetValue("InboxProcessor:PollIntervalSeconds", 5));
    private readonly int _batchSize = 
        configuration.GetValue("InboxProcessor:BatchSize", DefaultBatchSize);
    private readonly int _maxConcurrency = 
        configuration.GetValue("InboxProcessor:MaxConcurrency", DefaultMaxConcurrency);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Inbox processing batch failed.");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        // Fetch batch of pending messages with configurable size limit
        List<(Guid EventId, string EventType, string Payload)> pendingMessages;
        using (var scope = scopeFactory.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var pending = await unitOfWork.InboxMessageRepository.FindAsync(
                x => x.ProcessedAtUtc == null && x.Error == null,
                true,
                null,
                cancellationToken);

            pendingMessages = pending
                .Take(_batchSize)
                .Select(x => (x.EventId, x.EventType, x.Payload))
                .ToList();
        }

        if (pendingMessages.Count == 0)
            return;

        logger.LogInformation("Processing {Count} inbox messages.", pendingMessages.Count);

        // Process messages in parallel with concurrency limit
        using var semaphore = new SemaphoreSlim(_maxConcurrency, _maxConcurrency);
        var tasks = pendingMessages.Select(msg => ProcessOneWithSemaphoreAsync(msg, semaphore, cancellationToken));
        
        await Task.WhenAll(tasks);
    }

    private async Task ProcessOneWithSemaphoreAsync(
        (Guid EventId, string EventType, string Payload) message,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            await ProcessOneAsync(message.EventId, message.EventType, message.Payload, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task ProcessOneAsync(Guid eventId, string eventType, string payload, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Re-check to ensure message hasn't been processed by another worker
        var inbox = await unitOfWork.InboxMessageRepository.FirstOrDefaultAsync(
            x => x.EventId == eventId && x.ProcessedAtUtc == null && x.Error == null,
            false,
            null,
            cancellationToken);

        if (inbox is null)
            return;

        try
        {
            await HandleEventAsync(eventType, payload, mediator, cancellationToken);
            
            inbox.MarkProcessed();
            unitOfWork.InboxMessageRepository.Update(inbox);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully processed inbox message {EventId}.", eventId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process inbox event {EventId}.", eventId);
            await MarkFailedAsync(eventId, ex.Message, cancellationToken);
        }
    }

    private async Task HandleEventAsync(string eventType, string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var typeFullName = eventType.Contains('.') ? eventType : 
            EventTypeMap.Values.FirstOrDefault(v => v.EndsWith("." + eventType)) ?? eventType;

        switch (typeFullName)
        {
            case var t when t.EndsWith(nameof(UserRegisteredIntegrationEvent)):
                await HandleUserRegisteredAsync(payload, mediator, cancellationToken);
                break;
            
            case var t when t.EndsWith(nameof(UserConfirmedIntegrationEvent)):
                await HandleUserConfirmedAsync(payload, mediator, cancellationToken);
                break;

            case var t when t.EndsWith(nameof(OtpGeneratedIntegrationEvent)):
                await HandleOtpGeneratedAsync(payload, mediator, cancellationToken);
                break;

            default:
                throw new InvalidOperationException($"Unknown event type: {eventType}");
        }
    }

    private async Task HandleUserRegisteredAsync(string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(payload)
                  ?? throw new InvalidOperationException("Inbox payload could not be deserialized.");

        var command = new SendEmailCommand(
            evt.Email,
            "Welcome to GM Notifications Sample",
            $"Hello {evt.Username},\n\nThank you for registering!",
            UserId: evt.UserId);

        await mediator.Send(command, cancellationToken);
    }

    private async Task HandleUserConfirmedAsync(string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<UserConfirmedIntegrationEvent>(payload)
                  ?? throw new InvalidOperationException("Inbox payload could not be deserialized.");

        switch (evt.ConfirmationType)
        {
            case 0: // Email confirmation
            {
                var command = new SendEmailCommand(
                    evt.Subject,
                    "Email Confirmed",
                    $"Hello {evt.Subject},\n\nThank you for confirming email!",
                    UserId: evt.UserId);
                await mediator.Send(command, cancellationToken);
                break;
            }
            case 1: // SMS confirmation
            {
                var command = new SendSmsCommand(
                    evt.Subject,
                    "Phone Number Confirmed",
                    UserId: evt.UserId);
                await mediator.Send(command, cancellationToken);
                break;
            }
            case 2: // WhatsApp confirmation
            {
                var command = new SendWhatsAppCommand(
                    evt.Subject,
                    "Phone Number Confirmed",
                    UserId: evt.UserId);
                await mediator.Send(command, cancellationToken);
                break;
            }
            default:
                throw new InvalidOperationException($"Unknown confirmation type: {evt.ConfirmationType}");
        }
    }

    private async Task HandleOtpGeneratedAsync(string payload, IMediator mediator, CancellationToken cancellationToken)
    {
        var evt = JsonSerializer.Deserialize<OtpGeneratedIntegrationEvent>(payload)
                  ?? throw new InvalidOperationException("Inbox payload could not be deserialized.");

        // Channel carries the OTP delivery channel (GM.OTP OtpChannel: Sms = 1, Email = 2).
        // Message is already rendered by GM.OTP with the one-time code.
        switch (evt.Channel)
        {
            case 2: // Whatsapp
            {
                var command = new SendWhatsAppCommand(evt.Destination, evt.Message, UserId: evt.UserId);
                await mediator.Send(command, cancellationToken);
                break;
            }
            case 1: // SMS
            {
                var command = new SendSmsCommand(evt.Destination, evt.Message, UserId: evt.UserId);
                await mediator.Send(command, cancellationToken);
                break;
            }
            case 0: // Email
            {
                var command = new SendEmailCommand(
                    evt.Destination,
                    "Your one-time code",
                    evt.Message,
                    UserId: evt.UserId);
                await mediator.Send(command, cancellationToken);
                break;
            }
            default:
                throw new InvalidOperationException($"Unknown OTP channel: {evt.Channel}");
        }
    }

    private async Task MarkFailedAsync(Guid eventId, string error, CancellationToken cancellationToken)
    {
        // Fresh scope: the scope that just failed may have a polluted change tracker, so mark the
        // inbox row failed through a clean DbContext.
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var inbox = await unitOfWork.InboxMessageRepository.FirstOrDefaultAsync(
            x => x.EventId == eventId,
            false,
            null,
            cancellationToken);

        if (inbox is null)
            return;

        inbox.MarkFailed(error);
        unitOfWork.InboxMessageRepository.Update(inbox);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogWarning("Marked inbox message {EventId} as failed: {Error}", eventId, error);
    }
}
