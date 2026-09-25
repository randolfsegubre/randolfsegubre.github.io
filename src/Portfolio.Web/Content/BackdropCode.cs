namespace Portfolio.Web.Content;

/// <summary>
/// One column of scrolling code in the animated background.
/// </summary>
/// <param name="Code">The code shown. Generic idioms from the owner's own stack, nothing proprietary.</param>
/// <param name="HeroX">Left position in the hero, or null when the column is not used there.</param>
/// <param name="PageX">Left position in the faint page-wide layer.</param>
/// <param name="Seconds">How long one full scroll takes. Slow on purpose: ambient, not distracting.</param>
/// <param name="Reverse">True to scroll downward instead of upward, for variety.</param>
public sealed record CodeStream(string Code, string? HeroX, string PageX, int Seconds, bool Reverse);

/// <summary>
/// The code that scrolls in the animated background (ADR-0008).
/// </summary>
/// <remarks>
/// The background is purely decorative and hidden from assistive technology, so this text
/// is not prose and is not covered by the writing rules. It is chosen to show the owner's
/// real stack: C# and ASP.NET Core MVC, EF Core with MediatR, Razor, Vue.js, T-SQL, xUnit
/// and React, and Umbraco with a Content Security Policy nonce. Each column is rendered
/// twice in a row so the scroll can loop without a visible jump.
/// </remarks>
public static class BackdropCode
{
    public static IReadOnlyList<CodeStream> Streams { get; } =
    [
        new CodeStream(
            """
            using Microsoft.AspNetCore.Mvc;

            namespace Portfolio.Web.Controllers;

            public sealed class HomeController(
                IPortfolioContent content) : Controller
            {
                public IActionResult Index()
                {
                    var model = new PortfolioViewModel(
                        content.Profile,
                        content.Projects,
                        content.Experience,
                        content.Skills);

                    return View(model);
                }

                [Route("not-found")]
                public IActionResult NotFoundPage()
                    => View("NotFound", content.Profile);
            }
            """,
            HeroX: "-4%", PageX: "1%", Seconds: 64, Reverse: false),

        new CodeStream(
            """
            public sealed record GetAccountsQuery(Guid UserId)
                : IRequest<IReadOnlyList<AccountDto>>;

            public sealed class GetAccountsHandler(AppDbContext db)
                : IRequestHandler<GetAccountsQuery,
                    IReadOnlyList<AccountDto>>
            {
                public async Task<IReadOnlyList<AccountDto>> Handle(
                    GetAccountsQuery request, CancellationToken ct)
                {
                    return await db.Accounts
                        .Where(a => a.UserId == request.UserId)
                        .OrderBy(a => a.Name)
                        .Select(a => new AccountDto(
                            a.Id, a.Name, a.Balance))
                        .ToListAsync(ct);
                }
            }
            """,
            HeroX: "58%", PageX: "15%", Seconds: 78, Reverse: true),

        new CodeStream(
            """
            @model IReadOnlyList<Project>

            <section id="projects" class="section">
                <h2 class="section-title">Selected projects</h2>
                @foreach (var project in Model)
                {
                    <partial name="_ProjectCard"
                             model="project" />
                }
            </section>

            @inject IPortfolioContent Content
            <footer class="site-footer">
                &copy; @DateTime.UtcNow.Year
                @Content.Profile.Name
            </footer>
            """,
            HeroX: "68%", PageX: "29%", Seconds: 58, Reverse: false),

        new CodeStream(
            """
            <script setup>
            import { ref, computed } from 'vue'

            const bookings = ref([])
            const open = computed(() =>
              bookings.value.filter(b => !b.cancelled)
            )
            </script>

            <template>
              <ul class="bookings">
                <li v-for="b in open" :key="b.id">
                  {{ b.reference }} - {{ b.status }}
                </li>
              </ul>
            </template>
            """,
            HeroX: "79%", PageX: "43%", Seconds: 72, Reverse: true),

        new CodeStream(
            """
            SELECT TOP (50)
                   m.MemberId,
                   m.PlanCode,
                   e.EffectiveDate
            FROM dbo.Member AS m
            INNER JOIN dbo.Enrollment AS e
                    ON e.MemberId = m.MemberId
            WHERE e.Status = 'Active'
              AND e.EffectiveDate >= @Since
            ORDER BY e.EffectiveDate DESC;

            UPDATE dbo.Availability
               SET Remaining = Remaining - 1
             WHERE RoomId = @RoomId
               AND Remaining > 0;
            """,
            HeroX: "88%", PageX: "57%", Seconds: 66, Reverse: false),

        new CodeStream(
            """
            [Fact]
            public async Task Export_fails_loudly_when_a_page_fails()
            {
                using var broken = new HttpClient(
                    new StatusHandler(HttpStatusCode.InternalServerError));

                var error = await Assert.ThrowsAsync<
                    InvalidOperationException>(() =>
                        StaticExporter.ExportAsync(
                            broken, webRoot, output));

                Assert.Contains("500", error.Message);
            }

            const { data } = useQuery({
              queryKey: ['accounts'],
              queryFn: fetchAccounts,
            })
            """,
            HeroX: null, PageX: "71%", Seconds: 84, Reverse: true),

        new CodeStream(
            """
            @inherits UmbracoViewPage<HomePage>

            @foreach (var block in
                Model.Content.Children<ContentBlock>())
            {
                @await Html.PartialAsync(
                    "Blocks/" + block.ContentType.Alias,
                    block)
            }

            const nonce = document
              .querySelector('meta[name="csp-nonce"]')
              .content
            const script = document.createElement('script')
            script.nonce = nonce
            """,
            HeroX: null, PageX: "85%", Seconds: 70, Reverse: false),
    ];
}
