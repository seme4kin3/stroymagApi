using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAdvantagesAndComplectation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "advantages",
                schema: "stroymag",
                table: "products",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "complectation",
                schema: "stroymag",
                table: "products",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "advantages",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropColumn(
                name: "complectation",
                schema: "stroymag",
                table: "products");
        }
    }
}
