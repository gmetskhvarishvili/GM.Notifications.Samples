namespace GM.Notifications.Sample.API.Smses;

public sealed record SendSmsesBatchRequest(IReadOnlyList<SendSmsRequest> Items);
