namespace GM.Notifications.Sample.API.SlackMessages;

public sealed record SendSlackBatchRequest(IReadOnlyList<SendSlackRequest> Items);
