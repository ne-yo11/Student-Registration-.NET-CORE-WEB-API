using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Registration.Migrations
{
    /// <inheritdoc />
    public partial class isdeletedcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Courses SET isdeleted = 0 WHERE isdeleted IS NULL;");

            migrationBuilder.AlterColumn<bool>(
                name: "isdeleted",
                table: "Courses",
                nullable: false, 
                defaultValue: false); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
            name: "isdeleted",
            table: "Courses",
            nullable: true,
            defaultValue: null);

        }
    }
}
