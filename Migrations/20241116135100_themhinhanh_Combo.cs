using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_C_4.Migrations
{
    /// <inheritdoc />
    public partial class themhinhanh_Combo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Combos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Combos");
        }
    }
}
