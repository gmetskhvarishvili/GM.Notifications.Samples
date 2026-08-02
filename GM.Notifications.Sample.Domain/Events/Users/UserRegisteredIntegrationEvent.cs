using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.Notifications.Sample.Domain.Events.Users;

// MessageIdentity must match the alias on the publisher (GM.Identity) so Wolverine resolves this
// local type from the incoming cross-service message-type header.
[MessageIdentity("user.registered")]
public sealed record UserRegisteredIntegrationEvent(
    string? Email,
    string? Username,
    string? PhoneNumber) : IntegrationEvent;