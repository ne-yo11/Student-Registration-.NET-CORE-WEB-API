using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Registration.Migrations
{
    /// <inheritdoc />
    public partial class adddocumenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Documents",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "StudentDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocument_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocument_StudentId",
                table: "StudentDocument",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDocument");

            migrationBuilder.AddColumn<string>(
                name: "Documents",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
