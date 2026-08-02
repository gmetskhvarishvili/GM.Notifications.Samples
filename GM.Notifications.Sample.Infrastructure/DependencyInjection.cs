using GM.Messaging;
using GM.Notifications;
using GM.Notifications.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace GM.Notifications.Sample.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var notificationOptions = new NotificationOptions();
        configuration.GetSection("NotificationOptions").Bind(notificationOptions);
        services.AddSingleton(notificationOptions);

        // Add all GM Notifications services
        services.AddGMNotifications(configuration);

        return services;
    }

    public static IServiceCollection AddConsumerWorkerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGMMessaging(configuration);
        return services;
    }
}