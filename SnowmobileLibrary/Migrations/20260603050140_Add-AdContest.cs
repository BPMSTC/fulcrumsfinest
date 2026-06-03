using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnowmobileLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddAdContest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Subscribers");

            migrationBuilder.AddColumn<bool>(
                name: "AdContest",
                table: "Subscribers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdContest",
                table: "Subscribers");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Subscribers",
                type: "int",
                nullable: true);
        }
    }
}
