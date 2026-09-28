using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valetax.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommissionSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SchemaType = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfitEvents",
                columns: table => new
                {
                    ExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartnerExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Profit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SchemaType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    NextAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitEvents", x => x.ExternalId);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfitEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SchemaType = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commissions_ProfitEvents_ProfitEventId",
                        column: x => x.ProfitEventId,
                        principalTable: "ProfitEvents",
                        principalColumn: "ExternalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CommissionSettings",
                columns: new[] { "Id", "SchemaType", "UpdatedAt" },
                values: new object[] { 1, 0, null });

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_ProfitEventId_Level",
                table: "Commissions",
                columns: new[] { "ProfitEventId", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfitEvents_CreatedAt",
                table: "ProfitEvents",
                column: "CreatedAt",
                filter: "\"Status\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitEvents_NextAttemptAt",
                table: "ProfitEvents",
                column: "NextAttemptAt",
                filter: "\"Status\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitEvents_PartnerExternalId_CreatedAt",
                table: "ProfitEvents",
                columns: new[] { "PartnerExternalId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "CommissionSettings");

            migrationBuilder.DropTable(
                name: "ProfitEvents");
        }
    }
}