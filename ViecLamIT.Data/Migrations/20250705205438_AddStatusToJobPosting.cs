using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViecLamIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "JobPostings");

            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "JobPostings",
                newName: "AdminComment");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "JobPostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobPostings");

            migrationBuilder.RenameColumn(
                name: "AdminComment",
                table: "JobPostings",
                newName: "RejectionReason");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "JobPostings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
