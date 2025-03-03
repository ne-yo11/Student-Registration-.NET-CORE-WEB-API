using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Registration.Migrations
{
    /// <inheritdoc />
    public partial class studentisdeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "whendeleted",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "whendeleted",
                table: "Students");
        }
    }
}
