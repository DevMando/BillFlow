using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillFlow.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdateBillWebsite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Bills");
        }
    }
}
