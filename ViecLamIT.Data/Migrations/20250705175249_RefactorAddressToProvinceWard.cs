using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViecLamIT.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAddressToProvinceWard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "District",
                table: "Companies",
                newName: "Ward");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ward",
                table: "Companies",
                newName: "District");
        }
    }
}
