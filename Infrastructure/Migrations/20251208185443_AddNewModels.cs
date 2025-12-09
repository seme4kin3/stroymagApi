using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_products_ProductId",
                schema: "stroymag",
                table: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_products_Article",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_Has_Stock",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_Sku",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_product_images_ProductId_IsPrimary",
                schema: "stroymag",
                table: "product_images");

            migrationBuilder.DropIndex(
                name: "IX_categories_ParentId_Name",
                schema: "stroymag",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventory",
                schema: "stroymag",
                table: "inventory");

            migrationBuilder.DropColumn(
                name: "Unit",
                schema: "stroymag",
                table: "attribute_definitions");

            migrationBuilder.RenameTable(
                name: "inventory",
                schema: "stroymag",
                newName: "inventory_items",
                newSchema: "stroymag");

            migrationBuilder.RenameColumn(
                name: "Rrp",
                schema: "stroymag",
                table: "products",
                newName: "RecommendedRetailPrice");

            migrationBuilder.RenameColumn(
                name: "Has_Stock",
                schema: "stroymag",
                table: "products",
                newName: "HasStock");

            migrationBuilder.AlterColumn<bool>(
                name: "HasStock",
                schema: "stroymag",
                table: "products",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                schema: "stroymag",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "stroymag",
                table: "product_images",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "StoragePath",
                schema: "stroymag",
                table: "product_images",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                schema: "stroymag",
                table: "product_images",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                schema: "stroymag",
                table: "category_attributes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "stroymag",
                table: "categories",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "stroymag",
                table: "brands",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "stroymag",
                table: "attribute_definitions",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "stroymag",
                table: "inventory_items",
                type: "numeric(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldDefaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventory_items",
                schema: "stroymag",
                table: "inventory_items",
                column: "ProductId");

            migrationBuilder.CreateTable(
                name: "measurement_units",
                schema: "stroymag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurement_units", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_UnitId",
                schema: "stroymag",
                table: "products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_category_attributes_UnitId",
                schema: "stroymag",
                table: "category_attributes",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentId",
                schema: "stroymag",
                table: "categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_measurement_units_Symbol",
                schema: "stroymag",
                table: "measurement_units",
                column: "Symbol");

            migrationBuilder.AddForeignKey(
                name: "FK_category_attributes_measurement_units_UnitId",
                schema: "stroymag",
                table: "category_attributes",
                column: "UnitId",
                principalSchema: "stroymag",
                principalTable: "measurement_units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_items_products_ProductId",
                schema: "stroymag",
                table: "inventory_items",
                column: "ProductId",
                principalSchema: "stroymag",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_measurement_units_UnitId",
                schema: "stroymag",
                table: "products",
                column: "UnitId",
                principalSchema: "stroymag",
                principalTable: "measurement_units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_category_attributes_measurement_units_UnitId",
                schema: "stroymag",
                table: "category_attributes");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_items_products_ProductId",
                schema: "stroymag",
                table: "inventory_items");

            migrationBuilder.DropForeignKey(
                name: "FK_products_measurement_units_UnitId",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropTable(
                name: "measurement_units",
                schema: "stroymag");

            migrationBuilder.DropIndex(
                name: "IX_products_UnitId",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_category_attributes_UnitId",
                schema: "stroymag",
                table: "category_attributes");

            migrationBuilder.DropIndex(
                name: "IX_categories_ParentId",
                schema: "stroymag",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventory_items",
                schema: "stroymag",
                table: "inventory_items");

            migrationBuilder.DropColumn(
                name: "UnitId",
                schema: "stroymag",
                table: "products");

            migrationBuilder.DropColumn(
                name: "UnitId",
                schema: "stroymag",
                table: "category_attributes");

            migrationBuilder.RenameTable(
                name: "inventory_items",
                schema: "stroymag",
                newName: "inventory",
                newSchema: "stroymag");

            migrationBuilder.RenameColumn(
                name: "RecommendedRetailPrice",
                schema: "stroymag",
                table: "products",
                newName: "Rrp");

            migrationBuilder.RenameColumn(
                name: "HasStock",
                schema: "stroymag",
                table: "products",
                newName: "Has_Stock");

            migrationBuilder.AlterColumn<bool>(
                name: "Has_Stock",
                schema: "stroymag",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                schema: "stroymag",
                table: "product_images",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "StoragePath",
                schema: "stroymag",
                table: "product_images",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                schema: "stroymag",
                table: "product_images",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "stroymag",
                table: "categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "stroymag",
                table: "brands",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "stroymag",
                table: "attribute_definitions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                schema: "stroymag",
                table: "attribute_definitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                schema: "stroymag",
                table: "inventory",
                type: "numeric(18,3)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventory",
                schema: "stroymag",
                table: "inventory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_products_Article",
                schema: "stroymag",
                table: "products",
                column: "Article");

            migrationBuilder.CreateIndex(
                name: "IX_products_Has_Stock",
                schema: "stroymag",
                table: "products",
                column: "Has_Stock");

            migrationBuilder.CreateIndex(
                name: "IX_products_Sku",
                schema: "stroymag",
                table: "products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_images_ProductId_IsPrimary",
                schema: "stroymag",
                table: "product_images",
                columns: new[] { "ProductId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentId_Name",
                schema: "stroymag",
                table: "categories",
                columns: new[] { "ParentId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_products_ProductId",
                schema: "stroymag",
                table: "inventory",
                column: "ProductId",
                principalSchema: "stroymag",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
