using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wedding.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "weddPlans",
                columns: table => new
                {
                    WeddPlanId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WedderOne = table.Column<string>(nullable: false),
                    WedderTwo = table.Column<string>(nullable: false),
                    WeddingDate = table.Column<DateTime>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    WeddingPlanner = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weddPlans", x => x.WeddPlanId);
                });

            migrationBuilder.CreateTable(
                name: "rsvps",
                columns: table => new
                {
                    RSVPId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WeddPlanId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rsvps", x => x.RSVPId);
                    table.ForeignKey(
                        name: "FK_rsvps_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rsvps_weddPlans_WeddPlanId",
                        column: x => x.WeddPlanId,
                        principalTable: "weddPlans",
                        principalColumn: "WeddPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rsvps_UserId",
                table: "rsvps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_rsvps_WeddPlanId",
                table: "rsvps",
                column: "WeddPlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rsvps");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "weddPlans");
        }
    }
}
