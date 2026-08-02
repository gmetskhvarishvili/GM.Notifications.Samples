namespace GM.Notifications.Sample.API.SlackMessages;

public sealed record SendSlackRequest(
    string Channel,
    string Message,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);
