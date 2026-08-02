using GM.Notifications.Options;
using GM.Notifications.Sample.Application;
using GM.Notifications.Sample.Persistence;
using GM.Notifications.Sample.Persistence.Context;
using GM.Notifications.Sample.Worker.Workers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var notificationOptions = new NotificationOptions();
builder.Configuration.GetSection("NotificationOptions").Bind(notificationOptions);
builder.Services.AddSingleton(notificationOptions);
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHostedService<InboxProcessorWorker>();

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

app.Run();
