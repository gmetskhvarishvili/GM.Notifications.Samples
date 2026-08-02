namespace GM.Notifications.Sample.API.WhatsAppMessages;

public sealed record SendWhatsAppBatchRequest(IReadOnlyList<SendWhatsAppRequest> Items);
