using Asp.Versioning;
using GM.API.Controllers;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.SlackMessages.Commands.CancelSlack;
using GM.Notifications.Sample.Application.SlackMessages.Commands.SendSlack;
using GM.Notifications.Sample.Application.SlackMessages.Commands.SendSlackBatch;
using GM.Notifications.Sample.Application.SlackMessages.Queries.GetSlackMessageById;
using GM.Notifications.Sample.Application.SlackMessages.Queries.GetSlackMessagesList;
using Microsoft.AspNetCore.Mvc;

namespace GM.Notifications.Sample.API.SlackMessages;

/// <summary>
/// Slack Notifications Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications/slack-messages")]
public sealed class SlackNotificationsController : BaseController
{
    /// <summary>
    /// Send a Slack notification
    /// </summary>
    [HttpPost(Name = nameof(SendSlack))]
    [ProducesResponseType(typeof(SendSlackResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendSlack([FromBody] SendSlackRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendSlackCommand(
            request.Channel, request.Message,
            request.ScheduledAtUtc, request.CorrelationId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Send a batch of Slack notifications
    /// </summary>
    [HttpPost("batch", Name = nameof(SendSlackBatch))]
    [ProducesResponseType(typeof(SendSlackBatchResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendSlackBatch([FromBody] SendSlackBatchRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new SendSlackBatchItem(
            i.Channel, i.Message, i.ScheduledAtUtc, i.CorrelationId)).ToList();
        var result = await Mediator.Send(new SendSlackBatchCommand(items), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a Slack notification by id
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetSlackMessageById))]
    [ProducesResponseType(typeof(SlackNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSlackMessageById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSlackMessageByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get a list of Slack notifications
    /// </summary>
    [HttpGet(Name = nameof(GetSlackMessagesList))]
    [ProducesResponseType(typeof(GetSlackMessagesListResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlackMessagesList(
        [FromQuery] NotificationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetSlackMessagesListQuery(status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a pending Slack notification
    /// </summary>
    [HttpDelete("{id:guid}", Name = nameof(CancelSlack))]
    [ProducesResponseType(typeof(CancelSlackResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelSlack(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelSlackCommand(id), cancellationToken);
        return Ok(result);
    }
}
