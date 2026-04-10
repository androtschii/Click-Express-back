using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Доставка", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Доставка груза в течение 24 часов по всей стране", "https://placehold.co/400x400?text=Express", true, "Экспресс-доставка", 49.99m, 999 },
                    { 2, "Доставка", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Доставка груза в течение 3-5 рабочих дней", "https://placehold.co/400x400?text=Standard", true, "Стандартная доставка", 19.99m, 999 },
                    { 3, "Грузоперевозки", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Перевозка крупногабаритных и тяжёлых грузов", "https://placehold.co/400x400?text=Heavy", true, "Доставка негабарита", 199.99m, 50 },
                    { 4, "Международная", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Доставка грузов в 48 штатов США", "https://placehold.co/400x400?text=International", true, "Международная доставка", 299.99m, 100 },
                    { 5, "Спецперевозка", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Бережная перевозка хрупких и ценных грузов", "https://placehold.co/400x400?text=Fragile", true, "Хрупкий груз", 79.99m, 30 },
                    { 6, "Доставка", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Доставка груза с ночным временным слотом", "https://placehold.co/400x400?text=Night", false, "Ночная доставка", 89.99m, 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
