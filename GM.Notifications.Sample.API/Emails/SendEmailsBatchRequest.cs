namespace GM.Notifications.Sample.API.Emails;

public sealed record SendEmailsBatchRequest(IReadOnlyList<SendEmailRequest> Items);
