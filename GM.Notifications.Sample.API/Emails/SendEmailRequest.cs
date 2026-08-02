namespace GM.Notifications.Sample.API.Emails;

public sealed record SendEmailRequest(
    string To,
    string Subject,
    string Body,
    bool IsHtml = false,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);
