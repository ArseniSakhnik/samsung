using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mlsat.Migrations
{
    /// <inheritdoc />
    public partial class _6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Contamination",
                table: "Models",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Lof_Contamination",
                table: "Models",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Lof_NNeighbors",
                table: "Models",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contamination",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Lof_Contamination",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Lof_NNeighbors",
                table: "Models");
        }
    }
}
