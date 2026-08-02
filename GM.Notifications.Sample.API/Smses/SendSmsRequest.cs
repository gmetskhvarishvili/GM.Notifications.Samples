namespace GM.Notifications.Sample.API.Smses;

public sealed record SendSmsRequest(
    string PhoneNumber,
    string Body,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);
