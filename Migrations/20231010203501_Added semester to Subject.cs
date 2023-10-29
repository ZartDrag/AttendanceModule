using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCTest.Migrations
{
    /// <inheritdoc />
    public partial class AddedsemestertoSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Subjects");

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
