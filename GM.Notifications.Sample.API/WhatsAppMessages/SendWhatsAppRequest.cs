namespace GM.Notifications.Sample.API.WhatsAppMessages;

public sealed record SendWhatsAppRequest(
    string PhoneNumber,
    string Body,
    DateTime? ScheduledAtUtc = null,
    string? CorrelationId = null);
