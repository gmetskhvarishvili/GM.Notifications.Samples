using GM.API.Controllers;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.Pushes.Commands.CancelPush;
using GM.Notifications.Sample.Application.Pushes.Commands.SendPush;
using GM.Notifications.Sample.Application.Pushes.Commands.SendPushesBatch;
using GM.Notifications.Sample.Application.Pushes.Queries.GetPushById;
using GM.Notifications.Sample.Application.Pushes.Queries.GetPushesList;
using Microsoft.AspNetCore.Mvc;

namespace GM.Notifications.Sample.API.Pushes;

/// <summary>
/// Push Notifications Controller
/// </summary>
[ApiController]
[Route("api/notifications/pushes")]
public class PushNotificationsController : BaseController
{
    /// <summary>
    /// Send a push notification
    /// </summary>
    [HttpPost(Name = nameof(SendPush))]
    [ProducesResponseType(typeof(SendPushResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendPush([FromBody] SendPushRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendPushCommand(
            request.DeviceToken, request.Title, request.Body,
            request.Data, request.ScheduledAtUtc, request.CorrelationId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Send a batch of push notifications
    /// </summary>
    [HttpPost("batch", Name = nameof(SendPushesBatch))]
    [ProducesResponseType(typeof(SendPushesBatchResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendPushesBatch([FromBody] SendPushesBatchRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new SendPushBatchItem(
            i.DeviceToken, i.Title, i.Body, i.Data, i.ScheduledAtUtc, i.CorrelationId)).ToList();
        var result = await Mediator.Send(new SendPushesBatchCommand(items), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a push notification by id
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetPushById))]
    [ProducesResponseType(typeof(PushNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPushById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPushByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get a list of push notifications
    /// </summary>
    [HttpGet(Name = nameof(GetPushesList))]
    [ProducesResponseType(typeof(GetPushesListResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPushesList(
        [FromQuery] NotificationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetPushesListQuery(status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a pending push notification
    /// </summary>
    [HttpDelete("{id:guid}", Name = nameof(CancelPush))]
    [ProducesResponseType(typeof(CancelPushResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelPush(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelPushCommand(id), cancellationToken);
        return Ok(result);
    }
}
