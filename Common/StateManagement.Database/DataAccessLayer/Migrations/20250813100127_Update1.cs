using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StateManagement.Database.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessStates__CreateRequestId_BrandId",
                schema: "Orchestration",
                table: "ProcessStates");

            migrationBuilder.DropColumn(
                name: "AssociatedUserId",
                schema: "Orchestration",
                table: "ProcessStates");

            migrationBuilder.DropColumn(
                name: "BrandId",
                schema: "Orchestration",
                table: "ProcessStates");

            migrationBuilder.DropColumn(
                name: "ProcessBusinessStatusID",
                schema: "Orchestration",
                table: "ProcessStates");

            migrationBuilder.DropColumn(
                name: "RetriesCount",
                schema: "Orchestration",
                table: "ProcessStates");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStates__CreateRequestId_BrandId",
                schema: "Orchestration",
                table: "ProcessStates",
                column: "CreateRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessStates__CreateRequestId_BrandId",
                schema: "Orchestration",
                table: "ProcessStates");

            migrationBuilder.AddColumn<int>(
                name: "AssociatedUserId",
                schema: "Orchestration",
                table: "ProcessStates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "BrandId",
                schema: "Orchestration",
                table: "ProcessStates",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "ProcessBusinessStatusID",
                schema: "Orchestration",
                table: "ProcessStates",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "RetriesCount",
                schema: "Orchestration",
                table: "ProcessStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStates__CreateRequestId_BrandId",
                schema: "Orchestration",
                table: "ProcessStates",
                columns: new[] { "CreateRequestId", "BrandId" });
        }
    }
}
