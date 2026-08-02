using GM.Messaging.Domain.Events;
using Wolverine.Attributes;

namespace GM.Notifications.Sample.Domain.Events.Otp;

/// <summary>
/// GM.Notifications' copy of GM.OTP's OtpGenerated contract: a rendered one-time-code message to
/// deliver. Channel carries the OTP delivery channel (OtpChannel: Sms = 1, Email = 2).
/// MessageIdentity must match the alias on the publisher (GM.OTP) so Wolverine resolves this local
/// type from the incoming cross-service message-type header.
/// </summary>
[MessageIdentity("otp.generated")]
public sealed record OtpGeneratedIntegrationEvent(
    string Destination,
    int Channel,
    string Purpose,
    string Message) : IntegrationEvent;
