using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Valetax.Wallets.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "WalletPayouts",
                columns: table => new
                {
                    CommissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletPayouts", x => x.CommissionId);
                    table.CheckConstraint("CK_WalletPayouts_Amount", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_WalletPayouts_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletPayouts_WalletId_PaidAt",
                table: "WalletPayouts",
                columns: new[] { "WalletId", "PaidAt" })
                .Annotation("Npgsql:IndexInclude", new[] { "Amount" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletPayouts");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}