using Portfolio.Business.Models;

namespace Portfolio.Business.Content;

/// <summary>
/// Turns a tree of skills into an even spread over three spheres for the 3D cloud (ADR-0010).
/// </summary>
/// <remarks>
/// This is a pure function of its input, so the tests can check the geometry directly. The stars one step from the
/// centre go on an inner sphere, the stars two steps out on a middle one and everything further out on an outer
/// one. Each sphere is spread evenly with a golden-angle spiral, so no two stars crowd each other however the cloud is
/// turned. Stars are taken in tree order, so a branch lands in one band of latitude.
/// <para>
/// There is no flat drawing: with this many stars a flat map would be an unreadable pile. A visitor without script
/// gets every star and caption as a plain list, and script turns the same markup into the cloud.
/// </para>
/// </remarks>
public static class SkillUniverseLayout
{
    /// <summary>Radius in world units of the inner sphere, which holds the stars one step from the centre.</summary>
    public const double InnerRadius = 260;

    /// <summary>Radius in world units of the middle sphere, which holds the stars two steps from the centre.</summary>
    public const double MiddleRadius = 430;

    /// <summary>Radius in world units of the outer sphere, which holds every star three or more steps from the centre.</summary>
    public const double OuterRadius = 600;

    /// <summary>The deepest level the layout accepts. A deeper tree is a mistake in the data, not something to draw.</summary>
    public const int MaxDepth = 6;

    /// <summary>
    /// How far up or down the stars may go, as a fraction of the radius. Keeping clear of the poles stops stars bunching
    /// there, where a spiral is densest.
    /// </summary>
    private const double LatitudeLimit = 0.85;

    /// <summary>The golden angle in degrees: turning by it at each step spreads points evenly round a sphere.</summary>
    private const double GoldenAngleDegrees = 137.50776405;

    /// <summary>The sphere a star at this depth lives on.</summary>
    public static double RadiusFor(int depth) => depth switch
    {
        0 => 0,
        1 => InnerRadius,
        2 => MiddleRadius,
        _ => OuterRadius,
    };

    /// <summary>
    /// Places every star and collects every line.
    /// </summary>
    /// <param name="intro">The line shown above the map.</param>
    /// <param name="stars">The tree: exactly one star with no parent, every other star pointing at its parent.</param>
    /// <param name="crossLinks">Extra lines between stars that are related but not parent and child.</param>
    /// <param name="journey">The visiting order for the walk; must list every star exactly once.</param>
    public static UniverseMap Build(
        string intro,
        IReadOnlyList<SkillStar> stars,
        IReadOnlyList<SkillLink> crossLinks,
        IReadOnlyList<string> journey)
    {
        // STEP 1 of 4: index the stars and find the single centre. A broken tree fails loudly at start-up.
        var byId = stars.ToDictionary(s => s.Id, StringComparer.Ordinal);
        var roots = stars.Where(s => s.ParentId is null).ToList();
        if (roots.Count != 1)
        {
            throw new InvalidOperationException($"The skills map needs exactly one centre star, found {roots.Count}.");
        }

        foreach (var star in stars.Where(s => s.ParentId is not null))
        {
            if (!byId.ContainsKey(star.ParentId!))
            {
                throw new InvalidOperationException($"Star '{star.Id}' points at an unknown parent '{star.ParentId}'.");
            }
        }

        // STEP 2 of 4: work out how many steps each star is from the centre. A star that is its own ancestor never
        // reaches the centre, which is also reported as an error rather than looping forever.
        var depths = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var star in stars)
        {
            var depth = 0;
            for (var walker = star; walker.ParentId is not null; walker = byId[walker.ParentId])
            {
                depth++;
                if (depth > MaxDepth)
                {
                    throw new InvalidOperationException($"Star '{star.Id}' is deeper than the {MaxDepth} levels the layout supports.");
                }
            }

            depths[star.Id] = depth;
        }

        // STEP 3 of 4: spread each sphere's stars evenly, in tree order along a golden-angle spiral.
        var sphere = new Dictionary<string, (double Azimuth, double Elevation, double Radius)>(StringComparer.Ordinal);
        foreach (var level in stars.Where(s => depths[s.Id] > 0).GroupBy(s => RadiusFor(depths[s.Id])))
        {
            SpreadOverSphere(level.ToList(), level.Key);
        }

        void SpreadOverSphere(IReadOnlyList<SkillStar> onSphere, double radius)
        {
            for (var i = 0; i < onSphere.Count; i++)
            {
                // Evenly spaced heights make an even spread; the golden angle turns each star to a new side.
                var height = (1 - 2 * (i + 0.5) / onSphere.Count) * LatitudeLimit;
                sphere[onSphere[i].Id] = (
                    Math.Round(i * GoldenAngleDegrees % 360, 1),
                    Math.Round(Math.Asin(height) * 180 / Math.PI, 1),
                    radius);
            }
        }

        // STEP 4 of 4: draw a line from every star to its parent, then the extra cross links, and assemble the result.
        var links = stars
            .Where(s => s.ParentId is not null)
            .Select(s => new SkillLink(s.ParentId!, s.Id))
            .Concat(crossLinks)
            .ToList();

        var placed = stars
            .Select(s =>
            {
                var (azimuth, elevation, radius) = sphere.GetValueOrDefault(s.Id);
                return new PlacedStar(s, depths[s.Id], azimuth, elevation, radius);
            })
            .ToList();

        return new UniverseMap(intro, placed, links, journey);
    }
}
