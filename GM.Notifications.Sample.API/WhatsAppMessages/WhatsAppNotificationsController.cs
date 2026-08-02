using GM.API.Controllers;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.WhatsAppMessages.Commands.CancelWhatsApp;
using GM.Notifications.Sample.Application.WhatsAppMessages.Commands.SendWhatsApp;
using GM.Notifications.Sample.Application.WhatsAppMessages.Commands.SendWhatsAppBatch;
using GM.Notifications.Sample.Application.WhatsAppMessages.Queries.GetWhatsAppById;
using GM.Notifications.Sample.Application.WhatsAppMessages.Queries.GetWhatsAppMessagesList;
using Microsoft.AspNetCore.Mvc;

namespace GM.Notifications.Sample.API.WhatsAppMessages;

/// <summary>
/// WhatsApp Notifications Controller
/// </summary>
[ApiController]
[Route("api/notifications/whatsapp-messages")]
public class WhatsAppNotificationsController : BaseController
{
    /// <summary>
    /// Send a WhatsApp notification
    /// </summary>
    [HttpPost(Name = nameof(SendWhatsApp))]
    [ProducesResponseType(typeof(SendWhatsAppResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendWhatsApp([FromBody] SendWhatsAppRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendWhatsAppCommand(
            request.PhoneNumber, request.Body,
            request.ScheduledAtUtc, request.CorrelationId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Send a batch of WhatsApp notifications
    /// </summary>
    [HttpPost("batch", Name = nameof(SendWhatsAppBatch))]
    [ProducesResponseType(typeof(SendWhatsAppBatchResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendWhatsAppBatch([FromBody] SendWhatsAppBatchRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new SendWhatsAppBatchItem(
            i.PhoneNumber, i.Body, i.ScheduledAtUtc, i.CorrelationId)).ToList();
        var result = await Mediator.Send(new SendWhatsAppBatchCommand(items), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a WhatsApp notification by id
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetWhatsAppById))]
    [ProducesResponseType(typeof(WhatsAppNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWhatsAppById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetWhatsAppByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get a list of WhatsApp notifications
    /// </summary>
    [HttpGet(Name = nameof(GetWhatsAppMessagesList))]
    [ProducesResponseType(typeof(GetWhatsAppMessagesListResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWhatsAppMessagesList(
        [FromQuery] NotificationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetWhatsAppMessagesListQuery(status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a pending WhatsApp notification
    /// </summary>
    [HttpDelete("{id:guid}", Name = nameof(CancelWhatsApp))]
    [ProducesResponseType(typeof(CancelWhatsAppResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelWhatsApp(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelWhatsAppCommand(id), cancellationToken);
        return Ok(result);
    }
}
