using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Portfolio.Data;

/// <summary>
/// The SQL Server schema that holds the site's content (ADR-0012).
/// </summary>
/// <remarks>
/// Every table lives in the <c>content</c> schema, so the content is clearly apart from anything else in the
/// database. The schema is written out table by table below on purpose: it is the place to read what is allowed
/// (lengths, required columns, foreign keys, and the check constraints that stop an impossible state, such as two
/// rows in a one-row table or a status that is not a status). The only code that uses it is
/// <see cref="ContentRepository"/>, which the business layer calls: it fills the database from the JSON files and
/// reads it once at start-up, and nothing writes to it at request time (ADR-0013).
/// </remarks>
public sealed class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
    /// <summary>The name of the schema every table lives in.</summary>
    public const string Schema = "content";

    public DbSet<SiteSettingRow> SiteSettings => Set<SiteSettingRow>();

    public DbSet<ProfileRow> Profiles => Set<ProfileRow>();

    public DbSet<ProfileLinkRow> ProfileLinks => Set<ProfileLinkRow>();

    public DbSet<ContactChannelRow> ContactChannels => Set<ContactChannelRow>();

    public DbSet<StatRow> Stats => Set<StatRow>();

    public DbSet<ProjectRow> Projects => Set<ProjectRow>();

    public DbSet<ProjectLinkRow> ProjectLinks => Set<ProjectLinkRow>();

    public DbSet<LiveSiteRow> LiveSites => Set<LiveSiteRow>();

    public DbSet<RoleRow> Roles => Set<RoleRow>();

    public DbSet<SkillGroupRow> SkillGroups => Set<SkillGroupRow>();

    public DbSet<SkillUniverseRow> SkillUniverses => Set<SkillUniverseRow>();

    public DbSet<SkillStarRow> SkillStars => Set<SkillStarRow>();

    public DbSet<SkillLinkRow> SkillLinks => Set<SkillLinkRow>();

    public DbSet<JourneyStepRow> JourneySteps => Set<JourneyStepRow>();

    public DbSet<HeroTagRow> HeroTags => Set<HeroTagRow>();

    public DbSet<CodeStreamRow> CodeStreams => Set<CodeStreamRow>();

    public DbSet<TechLogoRow> TechLogos => Set<TechLogoRow>();

    public DbSet<SkillIconRow> SkillIcons => Set<SkillIconRow>();

    public DbSet<SocialIconRow> SocialIcons => Set<SocialIconRow>();

    /// <summary>
    /// Points a context at SQL Server the way this app always does, so the app, the tests and the design-time tools
    /// cannot differ. The migration history sits in the same schema as the content.
    /// </summary>
    /// <param name="options">The options being built.</param>
    /// <param name="connectionString">The connection string. It comes from configuration and is never kept in the repository.</param>
    public static void Configure(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlServer(connectionString, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", Schema));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        // STEP 1 of 4: the one-row tables. The check constraint makes a second row impossible, not just unlikely.
        modelBuilder.Entity<SiteSettingRow>(e =>
        {
            e.ToTable("SiteSetting", t =>
            {
                t.HasCheckConstraint("CK_SiteSetting_SingleRow", "[Id] = 1");
                t.HasCheckConstraint("CK_SiteSetting_HeroLogoCount", "[HeroLogoCount] >= 0");
            });
            e.Property(x => x.Url).HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.ShareDescription).HasMaxLength(1000);
        });

        modelBuilder.Entity<ProfileRow>(e =>
        {
            e.ToTable("Profile", t => t.HasCheckConstraint("CK_Profile_SingleRow", "[Id] = 1"));
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.Location).HasMaxLength(200);
            e.Property(x => x.Headline).HasMaxLength(2000);
            e.Property(x => x.Availability).HasMaxLength(500);
            e.Property(x => x.Email).HasMaxLength(320);
            e.Property(x => x.PhotoUrl).HasMaxLength(500);
            e.Property(x => x.PhotoAlt).HasMaxLength(500);
            e.Property(x => x.PhotoWebpUrl).HasMaxLength(500);
            e.Property(x => x.ResumeUrl).HasMaxLength(500);
            e.Property(x => x.Greeting).HasMaxLength(200);
        });

        modelBuilder.Entity<SkillUniverseRow>(e =>
        {
            e.ToTable("SkillUniverse", t => t.HasCheckConstraint("CK_SkillUniverse_SingleRow", "[Id] = 1"));
            e.Property(x => x.Intro).HasMaxLength(1000);
        });

        // STEP 2 of 4: the profile's lists, projects, live sites, roles and skill groups.
        modelBuilder.Entity<ProfileLinkRow>(e =>
        {
            e.ToTable("ProfileLink");
            e.HasKey(x => x.SortOrder);
            e.Property(x => x.Label).HasMaxLength(100);
            e.Property(x => x.Href).HasMaxLength(500);
        });

        modelBuilder.Entity<ContactChannelRow>(e =>
        {
            e.ToTable("ContactChannel");
            e.HasKey(x => x.SortOrder);
            e.Property(x => x.Label).HasMaxLength(100);
            e.Property(x => x.Display).HasMaxLength(100);
            e.Property(x => x.Href).HasMaxLength(500);
            e.Property(x => x.ActionLabel).HasMaxLength(100);
            e.Property(x => x.ActionHref).HasMaxLength(500);
        });

        modelBuilder.Entity<StatRow>(e =>
        {
            e.ToTable("Stat");
            e.HasKey(x => x.SortOrder);
            e.Property(x => x.Value).HasMaxLength(50);
            e.Property(x => x.Label).HasMaxLength(300);
        });

        modelBuilder.Entity<ProjectRow>(e =>
        {
            e.ToTable("Project", t => t.HasCheckConstraint("CK_Project_Status", "[Status] IN (N'inDevelopment', N'reference')"));
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Tagline).HasMaxLength(500);
            e.Property(x => x.Summary).HasMaxLength(2000);
            e.Property(x => x.Role).HasMaxLength(500);
            e.Property(x => x.Status).HasMaxLength(30);
            e.Property(x => x.HonestNote).HasMaxLength(1000);
            e.HasMany(x => x.Links).WithOne().HasForeignKey(l => l.ProjectId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProjectLinkRow>(e =>
        {
            e.ToTable("ProjectLink");
            e.HasKey(x => new { x.ProjectId, x.SortOrder });
            e.Property(x => x.ProjectId).HasMaxLength(100);
            e.Property(x => x.Label).HasMaxLength(100);
            e.Property(x => x.Href).HasMaxLength(500);
        });

        modelBuilder.Entity<LiveSiteRow>(e =>
        {
            e.ToTable("LiveSite");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Address).HasMaxLength(200);
            e.Property(x => x.Href).HasMaxLength(500);
            e.Property(x => x.Summary).HasMaxLength(2000);
            e.Property(x => x.Role).HasMaxLength(500);
            e.Property(x => x.HonestNote).HasMaxLength(1000);
        });

        modelBuilder.Entity<RoleRow>(e =>
        {
            e.ToTable("Role");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Company).HasMaxLength(200);
            e.Property(x => x.Client).HasMaxLength(200);
            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.Period).HasMaxLength(100);
            e.Property(x => x.Location).HasMaxLength(200);
        });

        modelBuilder.Entity<SkillGroupRow>(e =>
        {
            e.ToTable("SkillGroup");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Label).HasMaxLength(200);
        });

        // STEP 3 of 4: the skills map. The tree, the extra lines and the walk order all point at stars by foreign key,
        // so the database itself refuses a line to a star that does not exist or a star visited twice. The filtered
        // unique index allows at most one star with no parent: the centre.
        modelBuilder.Entity<SkillStarRow>(e =>
        {
            e.ToTable("SkillStar");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Label).HasMaxLength(100);
            e.Property(x => x.ParentId).HasMaxLength(100);
            e.Property(x => x.Caption).HasMaxLength(1000);
            e.Property(x => x.Icon).HasMaxLength(100);
            e.HasOne<SkillStarRow>().WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => x.ParentId).IsUnique().HasFilter("[ParentId] IS NULL").HasDatabaseName("UX_SkillStar_SingleCentre");
        });

        modelBuilder.Entity<SkillLinkRow>(e =>
        {
            e.ToTable("SkillLink", t => t.HasCheckConstraint("CK_SkillLink_NotToItself", "[FromId] <> [ToId]"));
            e.HasKey(x => x.SortOrder);
            e.Property(x => x.FromId).HasMaxLength(100);
            e.Property(x => x.ToId).HasMaxLength(100);
            e.HasOne<SkillStarRow>().WithMany().HasForeignKey(x => x.FromId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne<SkillStarRow>().WithMany().HasForeignKey(x => x.ToId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => new { x.FromId, x.ToId }).IsUnique();
        });

        modelBuilder.Entity<JourneyStepRow>(e =>
        {
            e.ToTable("JourneyStep");
            e.HasKey(x => x.SortOrder);
            e.Property(x => x.StarId).HasMaxLength(100);
            e.HasOne<SkillStarRow>().WithMany().HasForeignKey(x => x.StarId).OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => x.StarId).IsUnique();
        });

        // STEP 4 of 4: the decoration and the drawings.
        modelBuilder.Entity<HeroTagRow>(e =>
        {
            e.ToTable("HeroTag");
            e.HasKey(x => x.SortOrder);
            e.Property(x => x.Text).HasMaxLength(100);
        });

        modelBuilder.Entity<CodeStreamRow>(e =>
        {
            e.ToTable("CodeStream", t => t.HasCheckConstraint("CK_CodeStream_Seconds", "[Seconds] > 0"));
            e.HasKey(x => x.SortOrder);
        });

        modelBuilder.Entity<TechLogoRow>(e =>
        {
            e.ToTable("TechLogo", t => t.HasCheckConstraint("CK_TechLogo_Hex", "[Hex] LIKE N'#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]'"));
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Hex).HasMaxLength(7);
        });

        modelBuilder.Entity<SkillIconRow>(e =>
        {
            e.ToTable("SkillIcon");
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.Fill).HasMaxLength(7);
        });

        modelBuilder.Entity<SocialIconRow>(e =>
        {
            e.ToTable("SocialIcon", t => t.HasCheckConstraint("CK_SocialIcon_OneSource", "([LogoKey] IS NOT NULL AND [Markup] IS NULL) OR ([LogoKey] IS NULL AND [Markup] IS NOT NULL)"));
            e.HasKey(x => x.Label);
            e.Property(x => x.Label).HasMaxLength(100);
            e.HasIndex(x => x.SortOrder).IsUnique();
            e.Property(x => x.LogoKey).HasMaxLength(100);
            e.HasOne<TechLogoRow>().WithMany().HasForeignKey(x => x.LogoKey).OnDelete(DeleteBehavior.NoAction);
        });

        // Every whole-number key here is chosen by the content (a display position, or the fixed 1 of a one-row table),
        // never handed out by the database, so none of them is an IDENTITY column.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.FindPrimaryKey()?.Properties.Where(p => p.ClrType == typeof(int)) ?? [])
            {
                property.ValueGenerated = ValueGenerated.Never;
            }
        }
    }
}
