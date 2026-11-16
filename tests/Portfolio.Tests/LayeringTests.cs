using System.Reflection;
using Portfolio.Business.Services;
using Portfolio.Data;

namespace Portfolio.Tests;

/// <summary>
/// Keeps the three layers apart (ADR-0013): presentation (Portfolio.Api), business (Portfolio.Business) and data
/// access (Portfolio.Data), with every dependency pointing one way: presentation to business to data.
/// </summary>
/// <remarks>
/// The project references already make most violations impossible to compile. These tests check what the compiler
/// actually emitted, so they also catch the quieter leaks: a layer reaching the database through a package it gets
/// transitively, or a project reference added "just this once". If one of these fails, the fix is to move the code to
/// the layer it belongs to, never to loosen the test.
/// </remarks>
public sealed class LayeringTests
{
    private static readonly Assembly Presentation = typeof(Program).Assembly;
    private static readonly Assembly Business = typeof(ContentService).Assembly;
    private static readonly Assembly Data = typeof(PortfolioDbContext).Assembly;

    private static List<string> ReferencesOf(Assembly assembly) =>
        [.. assembly.GetReferencedAssemblies().Select(name => name.Name!)];

    private static bool IsDatabaseCode(string assemblyName) =>
        assemblyName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
        || assemblyName.StartsWith("Microsoft.Data.SqlClient", StringComparison.Ordinal)
        || assemblyName.StartsWith("System.Data", StringComparison.Ordinal);

    [Fact]
    public void The_presentation_layer_talks_to_the_business_layer_and_never_to_the_data_layer()
    {
        var references = ReferencesOf(Presentation);

        Assert.Contains("Portfolio.Business", references);
        Assert.DoesNotContain("Portfolio.Data", references);
        Assert.DoesNotContain(references, IsDatabaseCode);
    }

    [Fact]
    public void The_business_layer_uses_the_data_layer_but_knows_nothing_of_the_presentation_layer_or_the_database_itself()
    {
        var references = ReferencesOf(Business);

        Assert.Contains("Portfolio.Data", references);
        Assert.DoesNotContain("Portfolio.Api", references);
        Assert.DoesNotContain(references, IsDatabaseCode);
    }

    [Fact]
    public void The_data_layer_references_no_other_layer()
    {
        var references = ReferencesOf(Data);

        Assert.DoesNotContain("Portfolio.Business", references);
        Assert.DoesNotContain("Portfolio.Api", references);
    }

    [Fact]
    public void Only_the_data_layer_holds_the_schema_and_its_migrations()
    {
        Assert.Contains(Data.GetTypes(), t => t.Name == "InitialCreate");
        Assert.DoesNotContain(Business.GetTypes(), t => typeof(Microsoft.EntityFrameworkCore.DbContext).IsAssignableFrom(t));
        Assert.DoesNotContain(Presentation.GetTypes(), t => typeof(Microsoft.EntityFrameworkCore.DbContext).IsAssignableFrom(t));
    }
}
