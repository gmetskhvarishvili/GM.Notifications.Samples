namespace GM.Notifications.Sample.API.Pushes;

public sealed record SendPushRequest(
    string DeviceToken,
    string Title,
    string Body,
    string? Data = null,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);
