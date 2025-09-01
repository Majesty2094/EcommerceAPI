using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MajesticEcommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Product",
                table: "Transactions",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Transactions",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Transactions",
                newName: "Product");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "Price");
        }
    }
}
