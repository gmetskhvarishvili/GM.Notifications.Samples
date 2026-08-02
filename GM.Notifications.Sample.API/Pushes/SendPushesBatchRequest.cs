namespace GM.Notifications.Sample.API.Pushes;

public sealed record SendPushesBatchRequest(IReadOnlyList<SendPushRequest> Items);
