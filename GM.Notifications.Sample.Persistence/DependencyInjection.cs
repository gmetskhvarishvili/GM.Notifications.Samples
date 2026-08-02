using GM.Messaging.Persistence;
using GM.Messaging.Persistence.Inbox;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate;
using GM.Notifications.Sample.Domain.BoundedContext.MessageBoundedContext.InboxMessageAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.PushNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SlackMessageAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.WhatsAppNotificationAggregate.Interfaces;
using GM.Notifications.Sample.Domain.SeedWork;
using GM.Notifications.Sample.Persistence.Context;
using GM.Notifications.Sample.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GM.Notifications.Sample.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkNpgsql();

        services.AddDbContextPool<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                });
            options.UseInternalServiceProvider(serviceProvider);
        });

        AddDI(services);

        return services;
    }

    public static IServiceCollection AddConsumerWorkerPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                });
        });

        AddDI(services);

        return services;
    }

    private static void AddDI(IServiceCollection services)
    {
        services.AddTransient<IEmailNotificationRepository, EmailNotificationRepository>();
        services.AddTransient<ISmsNotificationRepository, SmsNotificationRepository>();
        services.AddTransient<IWhatsAppNotificationRepository, WhatsAppNotificationRepository>();
        services.AddTransient<IPushNotificationRepository, PushNotificationRepository>();
        services.AddTransient<ISlackNotificationRepository, SlackNotificationRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<InboxMessageRepository>();
        services.AddTransient<IInboxStore<InboxMessage>, InboxMessageRepository>();
        services.AddGMInboxProcessor<InboxMessage>();
        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();
    }
}