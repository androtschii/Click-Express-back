using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSeedToMatchLoads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Full Load", "Flatbed / Oversized Containers", "/images/real9.jpg", "Colorado Springs → Tampa", 4100m, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Full Load", "Flatbed / HVAC Units", "/images/real2.jpg", "Madera → Fort Collins", 5120m, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Full Load", "Flatbed / Steel Beams", "/images/real3.jpg", "Deer Park → Jackson", 2100m, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Partial", "Stepdeck / Heavy Machinery", "/images/real4.jpg", "Salt Lake City → Houston", 5500m, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Full Load", "Flatbed / Equipment", "/images/real5.jpg", "Key Largo → Lake Ozark", 4000m, 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock" },
                values: new object[] { "Military Load", "Military / Multi-Stop", "/images/real6.jpg", true, "Anniston → Apache Junction", 19499m, 1 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 7, "Full Load", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Flatbed / Heavy Equipment", "/images/real7.jpg", true, "Connellsville → Snyder", 3100m, 1 },
                    { 8, "Partial", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Flatbed / Construction Equipment", "/images/real8.jpg", true, "Las Vegas → Carnesville", 6350m, 1 },
                    { 9, "Full Load", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Flatbed / Multi-Stop Steel", "/images/real1.jpg", true, "Houston → Jackson", 14900m, 1 },
                    { 10, "Full Load", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Flatbed / Heavy Equipment", "/images/real10.jpg", true, "Phoenix → Memphis", 6800m, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Доставка", "Доставка груза в течение 24 часов по всей стране", "https://placehold.co/400x400?text=Express", "Экспресс-доставка", 49.99m, 999 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Доставка", "Доставка груза в течение 3-5 рабочих дней", "https://placehold.co/400x400?text=Standard", "Стандартная доставка", 19.99m, 999 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Грузоперевозки", "Перевозка крупногабаритных и тяжёлых грузов", "https://placehold.co/400x400?text=Heavy", "Доставка негабарита", 199.99m, 50 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Международная", "Доставка грузов в 48 штатов США", "https://placehold.co/400x400?text=International", "Международная доставка", 299.99m, 100 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[] { "Спецперевозка", "Бережная перевозка хрупких и ценных грузов", "https://placehold.co/400x400?text=Fragile", "Хрупкий груз", 79.99m, 30 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock" },
                values: new object[] { "Доставка", "Доставка груза с ночным временным слотом", "https://placehold.co/400x400?text=Night", false, "Ночная доставка", 89.99m, 20 });
        }
    }
}
