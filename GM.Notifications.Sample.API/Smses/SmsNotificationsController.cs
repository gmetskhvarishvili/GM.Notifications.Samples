using Asp.Versioning;
using GM.API.Controllers;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.Smses.Commands.CancelSms;
using GM.Notifications.Sample.Application.Smses.Commands.SendSms;
using GM.Notifications.Sample.Application.Smses.Commands.SendSmsesBatch;
using GM.Notifications.Sample.Application.Smses.Queries.GetSmsById;
using GM.Notifications.Sample.Application.Smses.Queries.GetSmsesList;
using Microsoft.AspNetCore.Mvc;

namespace GM.Notifications.Sample.API.Smses;

/// <summary>
/// SMS Notifications Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications/smses")]
public sealed class SmsNotificationsController : BaseController
{
    /// <summary>
    /// Send an SMS notification
    /// </summary>
    [HttpPost(Name = nameof(SendSms))]
    [ProducesResponseType(typeof(SendSmsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendSms([FromBody] SendSmsRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendSmsCommand(
            request.PhoneNumber, request.Body,
            request.ScheduledAtUtc, request.CorrelationId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Send a batch of SMS notifications
    /// </summary>
    [HttpPost("batch", Name = nameof(SendSmsesBatch))]
    [ProducesResponseType(typeof(SendSmsesBatchResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendSmsesBatch([FromBody] SendSmsesBatchRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new SendSmsBatchItem(
            i.PhoneNumber, i.Body, i.ScheduledAtUtc, i.CorrelationId)).ToList();
        var result = await Mediator.Send(new SendSmsesBatchCommand(items), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get an SMS notification by id
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetSmsById))]
    [ProducesResponseType(typeof(SmsNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSmsById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSmsByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get a list of SMS notifications
    /// </summary>
    [HttpGet(Name = nameof(GetSmsesList))]
    [ProducesResponseType(typeof(GetSmsesListResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSmsesList(
        [FromQuery] NotificationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetSmsesListQuery(status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a pending SMS notification
    /// </summary>
    [HttpDelete("{id:guid}", Name = nameof(CancelSms))]
    [ProducesResponseType(typeof(CancelSmsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelSms(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelSmsCommand(id), cancellationToken);
        return Ok(result);
    }
}
