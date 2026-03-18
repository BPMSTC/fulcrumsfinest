using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowmobileLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Subscribers_VSCA",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Subscribers_VSCA",
                table: "Subscriptions",
                column: "VSCA",
                principalTable: "Subscribers",
                principalColumn: "VSCA",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Subscribers_VSCA",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Subscribers_VSCA",
                table: "Subscriptions",
                column: "VSCA",
                principalTable: "Subscribers",
                principalColumn: "VSCA",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
