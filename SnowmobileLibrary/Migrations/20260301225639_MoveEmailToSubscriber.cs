using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowmobileLibrary.Migrations
{
    /// <inheritdoc />
    public partial class MoveEmailToSubscriber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscribers_Emails_EmailId",
                table: "Subscribers");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_EmailId",
                table: "Subscribers");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "Subscribers");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Subscribers",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Subscribers");

            migrationBuilder.AddColumn<int>(
                name: "EmailId",
                table: "Subscribers",
                type: "int",
                nullable: true);

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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_EmailId",
                table: "Subscribers",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_VSCA",
                table: "Emails",
                column: "VSCA");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribers_Emails_EmailId",
                table: "Subscribers",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "EmailId");
        }
    }
}
