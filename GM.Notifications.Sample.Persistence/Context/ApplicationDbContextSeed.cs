using Microsoft.Extensions.Logging;

namespace GM.Notifications.Sample.Persistence.Context;

/// <summary>
/// Extension point for post-migration data seeding. This sample has no seed data yet; the method
/// is a documented no-op so <c>Program.cs</c> has a stable call site to extend when it does.
/// Not a static class: <see cref="ApplicationDbContextSeed"/> itself is used as the generic
/// argument to <see cref="ILogger{TCategoryName}"/>, which disallows static types.
/// </summary>
public sealed class ApplicationDbContextSeed
{
    private ApplicationDbContextSeed()
    {
    }

    public static Task SeedAsync(ApplicationDbContext context, ILogger<ApplicationDbContextSeed> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        return Task.CompletedTask;
    }
}