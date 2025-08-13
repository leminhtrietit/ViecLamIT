using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViecLamIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndSpecializationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "DesiredPosition",
                table: "Profiles",
                newName: "DesiredSpecialization");

            migrationBuilder.AddColumn<string>(
                name: "DesiredCategory",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredCategory",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "JobPostings");

            migrationBuilder.RenameColumn(
                name: "DesiredSpecialization",
                table: "Profiles",
                newName: "DesiredPosition");

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
