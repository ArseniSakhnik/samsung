using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mlsat.Migrations
{
    /// <inheritdoc />
    public partial class reload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelColumn_Models_BaseModelId",
                table: "ModelColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceWeatherColumn_Models_BaseModelId",
                table: "SpaceWeatherColumn");

            migrationBuilder.DropColumn(
                name: "Algorithm",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Contamination",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Lof_Contamination",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Lof_NNeighbors",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "NNeighbors",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Percentile",
                table: "Models");

            migrationBuilder.RenameColumn(
                name: "BaseModelId",
                table: "SpaceWeatherColumn",
                newName: "ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_SpaceWeatherColumn_BaseModelId",
                table: "SpaceWeatherColumn",
                newName: "IX_SpaceWeatherColumn_ModelId");

            migrationBuilder.RenameColumn(
                name: "BaseModelId",
                table: "ModelColumn",
                newName: "ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelColumn_BaseModelId",
                table: "ModelColumn",
                newName: "IX_ModelColumn_ModelId");

            migrationBuilder.AddColumn<int>(
                name: "ModelType",
                table: "Models",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelColumn_Models_ModelId",
                table: "ModelColumn",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceWeatherColumn_Models_ModelId",
                table: "SpaceWeatherColumn",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelColumn_Models_ModelId",
                table: "ModelColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_SpaceWeatherColumn_Models_ModelId",
                table: "SpaceWeatherColumn");

            migrationBuilder.DropColumn(
                name: "ModelType",
                table: "Models");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "SpaceWeatherColumn",
                newName: "BaseModelId");

            migrationBuilder.RenameIndex(
                name: "IX_SpaceWeatherColumn_ModelId",
                table: "SpaceWeatherColumn",
                newName: "IX_SpaceWeatherColumn_BaseModelId");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "ModelColumn",
                newName: "BaseModelId");

            migrationBuilder.RenameIndex(
                name: "IX_ModelColumn_ModelId",
                table: "ModelColumn",
                newName: "IX_ModelColumn_BaseModelId");

            migrationBuilder.AddColumn<string>(
                name: "Algorithm",
                table: "Models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Contamination",
                table: "Models",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Models",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<int>(
                name: "NNeighbors",
                table: "Models",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Percentile",
                table: "Models",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelColumn_Models_BaseModelId",
                table: "ModelColumn",
                column: "BaseModelId",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaceWeatherColumn_Models_BaseModelId",
                table: "SpaceWeatherColumn",
                column: "BaseModelId",
                principalTable: "Models",
                principalColumn: "Id");
        }
    }
}
