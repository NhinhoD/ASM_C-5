using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_C_4.Migrations
{
    /// <inheritdoc />
    public partial class CheckoutCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCombo",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCombo",
                table: "OrderDetails");
        }
    }
}
