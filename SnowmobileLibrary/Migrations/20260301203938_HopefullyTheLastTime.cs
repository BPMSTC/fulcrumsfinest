using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowmobileLibrary.Migrations
{
    /// <inheritdoc />
    public partial class HopefullyTheLastTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionId);
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
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    VSCA = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Contest = table.Column<bool>(type: "bit", nullable: false),
                    ManualMail = table.Column<bool>(type: "bit", nullable: false),
                    Commercial = table.Column<bool>(type: "bit", nullable: false),
                    DateJoined = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    EmailId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.VSCA);
                    table.ForeignKey(
                        name: "FK_Subscribers_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "EmailId");
                    table.ForeignKey(
                        name: "FK_Subscribers_Subscriptions_VSCA",
                        column: x => x.VSCA,
                        principalTable: "Subscriptions",
                        principalColumn: "SubscriptionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_VSCA",
                table: "Addresses",
                column: "VSCA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_VSCA",
                table: "Emails",
                column: "VSCA");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_EmailId",
                table: "Subscribers",
                column: "EmailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Subscribers_VSCA",
                table: "Addresses",
                column: "VSCA",
                principalTable: "Subscribers",
                principalColumn: "VSCA",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Subscribers_VSCA",
                table: "Emails",
                column: "VSCA",
                principalTable: "Subscribers",
                principalColumn: "VSCA",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Subscribers_VSCA",
                table: "Emails");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "Subscriptions");
        }
    }
}
