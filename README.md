<p align="center">
  <img src="icon.png" alt="GM.Notifications Samples" width="140" height="140" />
</p>

# GM.Notifications Samples

[![CI](https://github.com/gmetskhvarishvili/GM.Notifications.Samples/actions/workflows/ci.yml/badge.svg)](https://github.com/gmetskhvarishvili/GM.Notifications.Samples/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A layered **DDD + CQRS** sample that shows how to build a real multi-channel notification system on
the **GM.\*** ecosystem: **[GM.Notifications](https://www.nuget.org/packages/GM.Notifications)**
(Email / SMS / WhatsApp / Push / Slack channels + the persisted, retrying notification model),
**[GM.API](https://www.nuget.org/packages/GM.API)** (Web API startup, versioning, Swagger),
**[GM.Mediator](https://www.nuget.org/packages/GM.Mediator)** (CQRS dispatch),
**[GM.EntityFramework](https://www.nuget.org/packages/GM.EntityFramework)** (repository / unit of
work / auditing), **[GM.Messaging](https://www.nuget.org/packages/GM.Messaging)** (Wolverine +
RabbitMQ inbox) and **[GM.Exceptions](https://www.nuget.org/packages/GM.Exceptions)**. Targets
**.NET 10**, backed by **PostgreSQL**.

## What it demonstrates

- An **API** (`GM.Notifications.Sample.API`) that accepts notification requests per channel
  (send / send-batch / cancel commands and list / by-id queries) and **persists** them as
  `Pending` notifications — it does not send inline.
- **Per-channel worker services** (`EmailWorker`, `SmsWorker`, `WhatsAppWorker`, `PushWorker`,
  `SlackWorker`) that poll for due notifications, deliver them through the matching
  `GM.Notifications.<Channel>` sender, and drive the `NotificationBase` lifecycle
  (`IsReadyToSend` → `MarkSent` / `MarkFailed` with retry).
- A **Consumer.Worker** that subscribes to integration events over **RabbitMQ** (e.g.
  `user.registered`, `user.confirmed`, `otp.generated`) and turns them into notifications via an
  inbox — showing how notifications get created from other services' events.
- **CQRS** with GM.Mediator; **DDD** aggregates per channel (deriving from
  `GM.Notifications.Domain`'s `NotificationBase`); localized errors via GM.Exceptions.

## Architecture

```
GM.Notifications.Sample.Domain/          # channel aggregates + inbox, integration events
GM.Notifications.Sample.Application/      # CQRS commands + queries (GM.Mediator handlers)
GM.Notifications.Sample.Common/           # shared resources & localized exceptions
GM.Notifications.Sample.Infrastructure/   # channel sender wiring, messaging integration
GM.Notifications.Sample.Persistence/      # ApplicationDbContext, EF configs, migrations, seeding
GM.Notifications.Sample.API/              # HTTP API + composition root
GM.Notifications.Sample.EmailWorker/      # delivers pending Email notifications
GM.Notifications.Sample.SmsWorker/        # delivers pending SMS notifications
GM.Notifications.Sample.WhatsAppWorker/   # delivers pending WhatsApp notifications
GM.Notifications.Sample.PushWorker/       # delivers pending Push notifications
GM.Notifications.Sample.SlackWorker/      # delivers pending Slack notifications
GM.Notifications.Sample.Consumer.Worker/  # RabbitMQ consumer: events -> notifications
GM.Notifications.Sample.Worker/           # generic background worker
tests/GM.Notifications.Sample.Tests/      # xUnit tests for the notification aggregates
```

Dependencies flow inward: Domain has no infrastructure dependencies; Application depends on Domain;
Persistence and Infrastructure implement outward concerns; the API and workers are composition roots.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **PostgreSQL**, and **RabbitMQ** for the event consumer. Quick local instances:
  ```bash
  docker run --name gm-notif-db -e POSTGRES_PASSWORD=123456 -p 5432:5432 -d postgres
  docker run --name gm-rabbit -p 5672:5672 -p 15672:15672 -d rabbitmq:management
  ```

## Running

1. Check the connection string in each project's `appsettings.json`
   (`ConnectionStrings:ApplicationDatabase`). The API applies migrations and seeds on startup.
2. Run the API:
   ```bash
   dotnet run --project GM.Notifications.Sample.API
   ```
3. Run one or more channel workers to actually deliver queued notifications, e.g.:
   ```bash
   dotnet run --project GM.Notifications.Sample.EmailWorker
   ```
   Configure the channel first — e.g. the `Email` section (`SmtpHost`, `Username`, `Password`, …)
   in the EmailWorker's `appsettings.json`. The provided values are **local/example placeholders**;
   supply your own via user secrets or environment variables.

## Testing

```bash
dotnet test
```

## License

MIT — see [LICENSE](LICENSE).
