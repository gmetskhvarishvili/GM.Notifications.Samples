using GM.Messaging.Persistence;
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
        // Only our own provider settings: AddGMInbox (in AddDI) attaches GM.Messaging's save interceptors. No
        // UseInternalServiceProvider: EF then builds its own, with the snake_case naming convention's services in it.
        services.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                }).UseSnakeCaseNamingConvention();
        });

        AddDI(services, configuration);

        return services;
    }

    public static IServiceCollection AddConsumerWorkerPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Only our own provider settings: AddGMInbox (in AddDI) attaches GM.Messaging's save interceptors.
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ApplicationDatabase"),
                o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.CommandTimeout(60);
                }).UseSnakeCaseNamingConvention();
        });

        AddDI(services, configuration);

        return services;
    }

    private static void AddDI(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailNotificationRepository, EmailNotificationRepository>();
        services.AddTransient<ISmsNotificationRepository, SmsNotificationRepository>();
        services.AddTransient<IWhatsAppNotificationRepository, WhatsAppNotificationRepository>();
        services.AddTransient<IPushNotificationRepository, PushNotificationRepository>();
        services.AddTransient<ISlackNotificationRepository, SlackNotificationRepository>();
        services.AddTransient<IInboxMessageRepository, InboxMessageRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();
        // The inbox, GM.Messaging's save interceptors on ApplicationDbContext, and personal data from configuration:
        // the key ring when Messaging:SensitiveData:Keys is set (the consumer and inbox worker read [SensitiveData]
        // values in incoming events), shared with the Identity and OTP samples. Registered after the DbContext.
        services.AddGMInbox<InboxMessage, ApplicationDbContext>(configuration);
    }
}