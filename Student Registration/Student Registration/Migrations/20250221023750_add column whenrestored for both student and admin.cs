using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Registration.Migrations
{
    /// <inheritdoc />
    public partial class addcolumnwhenrestoredforbothstudentandadmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "whenrestored",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "whenrestored",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "whenrestored",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "whenrestored",
                table: "Courses");
        }
    }
}
