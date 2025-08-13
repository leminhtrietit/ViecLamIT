using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViecLamIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndustryToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");
        }
    }
}
