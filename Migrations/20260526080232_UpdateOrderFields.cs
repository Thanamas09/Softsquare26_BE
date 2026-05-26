using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Food_order_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
