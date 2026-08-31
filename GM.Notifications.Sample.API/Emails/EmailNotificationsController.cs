using Asp.Versioning;
using GM.API.Controllers;
using GM.Notifications.Domain.Enums;
using GM.Notifications.Sample.Application.Emails.Commands.CancelEmail;
using GM.Notifications.Sample.Application.Emails.Commands.SendEmail;
using GM.Notifications.Sample.Application.Emails.Commands.SendEmailsBatch;
using GM.Notifications.Sample.Application.Emails.Queries.GetEmailById;
using GM.Notifications.Sample.Application.Emails.Queries.GetEmailsList;
using Microsoft.AspNetCore.Mvc;

namespace GM.Notifications.Sample.API.Emails;

/// <summary>
/// Email Notifications Controller
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications/emails")]
public sealed class EmailNotificationsController : BaseController
{
    /// <summary>
    /// Send an email notification
    /// </summary>
    [HttpPost(Name = nameof(SendEmail))]
    [ProducesResponseType(typeof(SendEmailResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendEmailCommand(
            request.To, request.Subject, request.Body, request.IsHtml,
            request.ScheduledAtUtc, request.CorrelationId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Send a batch of email notifications
    /// </summary>
    [HttpPost("batch", Name = nameof(SendEmailsBatch))]
    [ProducesResponseType(typeof(SendEmailsBatchResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendEmailsBatch([FromBody] SendEmailsBatchRequest request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new SendEmailBatchItem(
            i.To, i.Subject, i.Body, i.IsHtml, i.ScheduledAtUtc, i.CorrelationId)).ToList();
        var result = await Mediator.Send(new SendEmailsBatchCommand(items), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get an email notification by id
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetEmailById))]
    [ProducesResponseType(typeof(EmailNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmailById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetEmailByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get a list of email notifications
    /// </summary>
    [HttpGet(Name = nameof(GetEmailsList))]
    [ProducesResponseType(typeof(GetEmailsListResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailsList(
        [FromQuery] NotificationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetEmailsListQuery(status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cancel a pending email notification
    /// </summary>
    [HttpDelete("{id:guid}", Name = nameof(CancelEmail))]
    [ProducesResponseType(typeof(CancelEmailResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelEmail(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelEmailCommand(id), cancellationToken);
        return Ok(result);
    }
}
