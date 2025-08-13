using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StateManagement.Database.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitOrchestration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Orchestration");

            migrationBuilder.CreateTable(
                name: "ProcessRequests",
                schema: "Orchestration",
                columns: table => new
                {
                    RequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExternalRequestId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRequests", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "ProcessResponses",
                schema: "Orchestration",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessResponses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_ProcessResponses_ProcessRequests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Orchestration",
                        principalTable: "ProcessRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessStates",
                schema: "Orchestration",
                columns: table => new
                {
                    ProcessStateId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessTypeId = table.Column<short>(type: "smallint", nullable: false),
                    BrandId = table.Column<short>(type: "smallint", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    AssociatedUserId = table.Column<int>(type: "int", nullable: true),
                    LockTimeoutInSeconds = table.Column<short>(type: "smallint", nullable: false),
                    ProcessBusinessStatusID = table.Column<byte>(type: "tinyint", nullable: false),
                    RetriesCount = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersionStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreateRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ProcessData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStates", x => x.ProcessStateId);
                    table.ForeignKey(
                        name: "FK_ProcessStates_ProcessRequests_CreateRequestId",
                        column: x => x.CreateRequestId,
                        principalSchema: "Orchestration",
                        principalTable: "ProcessRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStateRequests__ExternalRequestId",
                schema: "Orchestration",
                table: "ProcessRequests",
                column: "ExternalRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStateRequests__RequestDateTime",
                schema: "Orchestration",
                table: "ProcessRequests",
                column: "RequestDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessResponses_RequestId",
                schema: "Orchestration",
                table: "ProcessResponses",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStates__CreateRequestId_BrandId",
                schema: "Orchestration",
                table: "ProcessStates",
                columns: new[] { "CreateRequestId", "BrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStates__LastModifiedDateTime",
                schema: "Orchestration",
                table: "ProcessStates",
                column: "LastModifiedDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessResponses",
                schema: "Orchestration");

            migrationBuilder.DropTable(
                name: "ProcessStates",
                schema: "Orchestration");

            migrationBuilder.DropTable(
                name: "ProcessRequests",
                schema: "Orchestration");
        }
    }
}
