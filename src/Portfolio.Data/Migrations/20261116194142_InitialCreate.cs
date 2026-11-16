using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "CodeStream",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seconds = table.Column<int>(type: "int", nullable: false),
                    Reverse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeStream", x => x.SortOrder);
                    table.CheckConstraint("CK_CodeStream_Seconds", "[Seconds] > 0");
                });

            migrationBuilder.CreateTable(
                name: "ContactChannel",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Display = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Href = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActionLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionHref = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactChannel", x => x.SortOrder);
                });

            migrationBuilder.CreateTable(
                name: "HeroTag",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroTag", x => x.SortOrder);
                });

            migrationBuilder.CreateTable(
                name: "LiveSite",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Href = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HonestNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Highlights = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stack = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveSite", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Availability = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoAlt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoWebpUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResumeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Greeting = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignatureStack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackdropTags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    About = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                    table.CheckConstraint("CK_Profile_SingleRow", "[Id] = 1");
                });

            migrationBuilder.CreateTable(
                name: "ProfileLink",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Href = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileLink", x => x.SortOrder);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Featured = table.Column<bool>(type: "bit", nullable: false),
                    HonestNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Highlights = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stack = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.CheckConstraint("CK_Project_Status", "[Status] IN (N'inDevelopment', N'reference')");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Client = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Period = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Highlights = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stack = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSetting",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ShareDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HeroLogoCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSetting", x => x.Id);
                    table.CheckConstraint("CK_SiteSetting_HeroLogoCount", "[HeroLogoCount] >= 0");
                    table.CheckConstraint("CK_SiteSetting_SingleRow", "[Id] = 1");
                });

            migrationBuilder.CreateTable(
                name: "SkillGroup",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Items = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillIcon",
                schema: "content",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Markup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fill = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    OnDark = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillIcon", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "SkillStar",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Caption = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Main = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillStar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillStar_SkillStar_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "content",
                        principalTable: "SkillStar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SkillUniverse",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Intro = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillUniverse", x => x.Id);
                    table.CheckConstraint("CK_SkillUniverse_SingleRow", "[Id] = 1");
                });

            migrationBuilder.CreateTable(
                name: "Stat",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stat", x => x.SortOrder);
                });

            migrationBuilder.CreateTable(
                name: "TechLogo",
                schema: "content",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Hex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Markup = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechLogo", x => x.Key);
                    table.CheckConstraint("CK_TechLogo_Hex", "[Hex] LIKE N'#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]'");
                });

            migrationBuilder.CreateTable(
                name: "ProjectLink",
                schema: "content",
                columns: table => new
                {
                    ProjectId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Href = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLink", x => new { x.ProjectId, x.SortOrder });
                    table.ForeignKey(
                        name: "FK_ProjectLink_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "content",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JourneyStep",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    StarId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JourneyStep", x => x.SortOrder);
                    table.ForeignKey(
                        name: "FK_JourneyStep_SkillStar_StarId",
                        column: x => x.StarId,
                        principalSchema: "content",
                        principalTable: "SkillStar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SkillLink",
                schema: "content",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    FromId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLink", x => x.SortOrder);
                    table.CheckConstraint("CK_SkillLink_NotToItself", "[FromId] <> [ToId]");
                    table.ForeignKey(
                        name: "FK_SkillLink_SkillStar_FromId",
                        column: x => x.FromId,
                        principalSchema: "content",
                        principalTable: "SkillStar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SkillLink_SkillStar_ToId",
                        column: x => x.ToId,
                        principalSchema: "content",
                        principalTable: "SkillStar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SocialIcon",
                schema: "content",
                columns: table => new
                {
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LogoKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Markup = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialIcon", x => x.Label);
                    table.CheckConstraint("CK_SocialIcon_OneSource", "([LogoKey] IS NOT NULL AND [Markup] IS NULL) OR ([LogoKey] IS NULL AND [Markup] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SocialIcon_TechLogo_LogoKey",
                        column: x => x.LogoKey,
                        principalSchema: "content",
                        principalTable: "TechLogo",
                        principalColumn: "Key");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JourneyStep_StarId",
                schema: "content",
                table: "JourneyStep",
                column: "StarId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiveSite_SortOrder",
                schema: "content",
                table: "LiveSite",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_SortOrder",
                schema: "content",
                table: "Project",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_SortOrder",
                schema: "content",
                table: "Role",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillGroup_SortOrder",
                schema: "content",
                table: "SkillGroup",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillIcon_SortOrder",
                schema: "content",
                table: "SkillIcon",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillLink_FromId_ToId",
                schema: "content",
                table: "SkillLink",
                columns: new[] { "FromId", "ToId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillLink_ToId",
                schema: "content",
                table: "SkillLink",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillStar_SortOrder",
                schema: "content",
                table: "SkillStar",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SkillStar_SingleCentre",
                schema: "content",
                table: "SkillStar",
                column: "ParentId",
                unique: true,
                filter: "[ParentId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SocialIcon_LogoKey",
                schema: "content",
                table: "SocialIcon",
                column: "LogoKey");

            migrationBuilder.CreateIndex(
                name: "IX_SocialIcon_SortOrder",
                schema: "content",
                table: "SocialIcon",
                column: "SortOrder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechLogo_SortOrder",
                schema: "content",
                table: "TechLogo",
                column: "SortOrder",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeStream",
                schema: "content");

            migrationBuilder.DropTable(
                name: "ContactChannel",
                schema: "content");

            migrationBuilder.DropTable(
                name: "HeroTag",
                schema: "content");

            migrationBuilder.DropTable(
                name: "JourneyStep",
                schema: "content");

            migrationBuilder.DropTable(
                name: "LiveSite",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Profile",
                schema: "content");

            migrationBuilder.DropTable(
                name: "ProfileLink",
                schema: "content");

            migrationBuilder.DropTable(
                name: "ProjectLink",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SiteSetting",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SkillGroup",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SkillIcon",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SkillLink",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SkillUniverse",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SocialIcon",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Stat",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "content");

            migrationBuilder.DropTable(
                name: "SkillStar",
                schema: "content");

            migrationBuilder.DropTable(
                name: "TechLogo",
                schema: "content");
        }
    }
}
