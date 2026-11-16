using Microsoft.EntityFrameworkCore;

namespace Portfolio.Data;

/// <summary>
/// The business layer's only way into the content database (ADR-0013).
/// </summary>
/// <remarks>
/// It speaks in rows and in nothing else: it does not know what a content record is, what a page is, or that there is
/// a web at all. That is what makes the data layer replaceable (another database, or an in-memory fake in a test)
/// without touching the layers above.
/// </remarks>
public interface IContentRepository
{
    /// <summary>Creates the database if it is missing and applies every migration that has not been applied.</summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>Reads every row, or returns null when the database holds no content yet (it was never seeded).</summary>
    Task<ContentRows?> ReadAsync(CancellationToken cancellationToken = default);

    /// <summary>Replaces every row with the given ones, all or nothing.</summary>
    Task ReplaceAsync(ContentRows rows, CancellationToken cancellationToken = default);
}

/// <summary>
/// The SQL Server implementation of <see cref="IContentRepository"/> (ADR-0012).
/// </summary>
public sealed class ContentRepository(PortfolioDbContext db) : IContentRepository
{
    public Task MigrateAsync(CancellationToken cancellationToken = default) => db.Database.MigrateAsync(cancellationToken);

    /// <summary>
    /// Reads the whole site. Reads are untracked, because nothing read here is ever saved back, and every list comes
    /// back in display order.
    /// </summary>
    public async Task<ContentRows?> ReadAsync(CancellationToken cancellationToken = default)
    {
        // The one-row tables come first. If the site row is missing, nothing was ever seeded.
        var site = await db.SiteSettings.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
        if (site is null)
        {
            return null;
        }

        return new ContentRows
        {
            Site = site,
            Profile = await db.Profiles.AsNoTracking().SingleAsync(cancellationToken),
            SkillUniverse = await db.SkillUniverses.AsNoTracking().SingleAsync(cancellationToken),
            ProfileLinks = await db.ProfileLinks.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            ContactChannels = await db.ContactChannels.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            Stats = await db.Stats.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            Projects = await db.Projects.AsNoTracking().Include(p => p.Links.OrderBy(l => l.SortOrder)).OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            LiveSites = await db.LiveSites.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            Roles = await db.Roles.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            SkillGroups = await db.SkillGroups.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            SkillStars = await db.SkillStars.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            SkillLinks = await db.SkillLinks.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            JourneySteps = await db.JourneySteps.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            HeroTags = await db.HeroTags.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            CodeStreams = await db.CodeStreams.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            TechLogos = await db.TechLogos.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            SkillIcons = await db.SkillIcons.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
            SocialIcons = await db.SocialIcons.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken),
        };
    }

    /// <summary>
    /// Makes the database hold exactly the given rows, inside one transaction: either all of it changes or none of it
    /// does, so a failed write (a schema rule refusing a row, a lost connection) leaves the previous content in place.
    /// Running it twice with the same rows gives the same database.
    /// </summary>
    public async Task ReplaceAsync(ContentRows rows, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rows);

        // STEP 1 of 3: do it all in one transaction.
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        // STEP 2 of 3: empty every content table. Rows that point at others go first, so no foreign key is ever broken.
        await db.JourneySteps.ExecuteDeleteAsync(cancellationToken);
        await db.SkillLinks.ExecuteDeleteAsync(cancellationToken);
        await db.SkillStars.ExecuteDeleteAsync(cancellationToken);
        await db.SocialIcons.ExecuteDeleteAsync(cancellationToken);
        await db.TechLogos.ExecuteDeleteAsync(cancellationToken);
        await db.SkillIcons.ExecuteDeleteAsync(cancellationToken);
        await db.ProjectLinks.ExecuteDeleteAsync(cancellationToken);
        await db.Projects.ExecuteDeleteAsync(cancellationToken);
        await db.LiveSites.ExecuteDeleteAsync(cancellationToken);
        await db.Roles.ExecuteDeleteAsync(cancellationToken);
        await db.SkillGroups.ExecuteDeleteAsync(cancellationToken);
        await db.HeroTags.ExecuteDeleteAsync(cancellationToken);
        await db.CodeStreams.ExecuteDeleteAsync(cancellationToken);
        await db.ContactChannels.ExecuteDeleteAsync(cancellationToken);
        await db.ProfileLinks.ExecuteDeleteAsync(cancellationToken);
        await db.Stats.ExecuteDeleteAsync(cancellationToken);
        await db.Profiles.ExecuteDeleteAsync(cancellationToken);
        await db.SkillUniverses.ExecuteDeleteAsync(cancellationToken);
        await db.SiteSettings.ExecuteDeleteAsync(cancellationToken);

        // STEP 3 of 3: insert the new rows (Entity Framework works out the order from the foreign keys) and commit.
        db.ChangeTracker.Clear();
        db.AddRange(rows.AllRows());
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
