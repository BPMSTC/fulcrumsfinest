using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowmobileLibrary.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    VSCA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Contest = table.Column<bool>(type: "bit", nullable: false),
                    ManualMail = table.Column<bool>(type: "bit", nullable: false),
                    Commercial = table.Column<bool>(type: "bit", nullable: false),
                    DateJoined = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.VSCA);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VSCA = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Subscribers_VSCA",
                        column: x => x.VSCA,
                        principalTable: "Subscribers",
                        principalColumn: "VSCA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    EmailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VSCA = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.EmailId);
                    table.ForeignKey(
                        name: "FK_Emails_Subscribers_VSCA",
                        column: x => x.VSCA,
                        principalTable: "Subscribers",
                        principalColumn: "VSCA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VSCA = table.Column<int>(type: "int", nullable: false),
                    ExpDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DateRenewed = table.Column<DateOnly>(type: "date", nullable: false),
                    IssuesRemaining = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Subscribers_VSCA",
                        column: x => x.VSCA,
                        principalTable: "Subscribers",
                        principalColumn: "VSCA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_VSCA",
                table: "Addresses",
                column: "VSCA");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_VSCA",
                table: "Emails",
                column: "VSCA");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VSCA",
                table: "Subscriptions",
                column: "VSCA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Subscribers");
        }
    }
}
