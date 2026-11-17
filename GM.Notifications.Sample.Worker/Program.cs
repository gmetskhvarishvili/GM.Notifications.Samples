using GM.Messaging.Persistence;
using GM.Notifications.Options;
using GM.Notifications.Sample.Application;
using GM.Notifications.Sample.Persistence;
using GM.Notifications.Sample.Persistence.Context;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.Notifications.Sample.Domain.Events.Otp;
using GM.Notifications.Sample.Domain.Events.Users;
using GM.Notifications.Sample.Worker.InboxHandlers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var notificationOptions = new NotificationOptions();
builder.Configuration.GetSection("NotificationOptions").Bind(notificationOptions);
builder.Services.AddSingleton(notificationOptions);
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

// GM.Messaging's inbox worker handles the events the consumer worker ingested, one handler per event
// type (settings: Messaging:Inbox). AddPersistence's AddGMInbox(configuration) already set up the key ring
// for the stored events' encrypted [SensitiveData] values (Messaging:SensitiveData).
builder.Services.AddGMInboxWorker<InboxMessage, ApplicationDbContext>();
builder.Services.AddGMInboxHandler<UserRegisteredIntegrationEvent, UserRegisteredInboxHandler>();
builder.Services.AddGMInboxHandler<UserConfirmedIntegrationEvent, UserConfirmedInboxHandler>();
builder.Services.AddGMInboxHandler<OtpGeneratedIntegrationEvent, OtpGeneratedInboxHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (context != null && (await context.Database.GetPendingMigrationsAsync()).Any())
            await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

await app.RunAsync();
