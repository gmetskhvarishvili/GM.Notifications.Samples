using GM.Notifications.Domain.Enums;
using Xunit;
using EmailNotification = GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.EmailNotificationAggregate.EmailNotification;
using SmsNotification = GM.Notifications.Sample.Domain.BoundedContext.NotificationBoundedContext.SmsNotificationAggregate.SmsNotification;

namespace GM.Notifications.Sample.Tests;

// The sample aggregates (EmailNotification, SmsNotification, ...) derive from GM.Notifications'
// NotificationBase, so these tests exercise both the sample factories and the library lifecycle.
public class NotificationAggregateTests
{
    [Fact]
    public void EmailNotification_Create_StartsPendingAndReadyToSend()
    {
        var email = EmailNotification.Create(
            to: "user@example.com",
            subject: "Welcome",
            body: "<h1>Hi</h1>",
            isHtml: true,
            maxRetries: 3);

        Assert.Equal(NotificationStatus.Pending, email.Status);
        Assert.Equal(0, email.RetryCount);
        Assert.Equal("user@example.com", email.To);
        Assert.Equal("Welcome", email.Subject);
        Assert.True(email.IsHtml);
        Assert.True(email.IsReadyToSend(DateTime.UtcNow));
    }

    [Fact]
    public void EmailNotification_ScheduledInFuture_IsNotReadyYet()
    {
        var now = DateTime.UtcNow;
        var email = EmailNotification.Create(
            "user@example.com", "Later", "body", false, maxRetries: 3,
            scheduledAtUtc: now.AddHours(2));

        Assert.False(email.IsReadyToSend(now));
    }

    [Fact]
    public void MarkSent_MovesNotificationToSent()
    {
        var now = DateTime.UtcNow;
        var email = EmailNotification.Create("user@example.com", "s", "b", false, maxRetries: 3);

        email.MarkSent(now);

        Assert.Equal(NotificationStatus.Sent, email.Status);
        Assert.Equal(now, email.SentAtUtc);
        Assert.False(email.IsReadyToSend(now));
    }

    [Fact]
    public void MarkFailed_RetriesUntilMaxThenFails()
    {
        var now = DateTime.UtcNow;
        var sms = SmsNotification.Create("+995555000000", "code 1234", maxRetries: 2);

        sms.MarkFailed("carrier rejected", now);
        Assert.Equal(NotificationStatus.Pending, sms.Status);
        Assert.Equal(1, sms.RetryCount);
        Assert.True(sms.IsReadyToSend(now));

        sms.MarkFailed("carrier rejected again", now);
        Assert.Equal(NotificationStatus.Failed, sms.Status);
        Assert.Equal(2, sms.RetryCount);
        Assert.False(sms.IsReadyToSend(now));
    }

    [Fact]
    public void Cancel_MovesNotificationToCancelled()
    {
        var sms = SmsNotification.Create("+995555000000", "hi", maxRetries: 3);

        sms.Cancel();

        Assert.Equal(NotificationStatus.Cancelled, sms.Status);
        Assert.False(sms.IsReadyToSend(DateTime.UtcNow));
    }
}
