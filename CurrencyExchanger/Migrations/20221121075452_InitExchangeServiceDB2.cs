using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchanger.Migrations
{
    /// <inheritdoc />
    public partial class InitExchangeServiceDB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exchanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<long>(type: "bigint", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Historical = table.Column<bool>(type: "bit", nullable: false),
                    FromAmount = table.Column<decimal>(type: "decimal(20,7)", nullable: false),
                    ToAmount = table.Column<decimal>(type: "decimal(20,7)", nullable: false),
                    From = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    Succeded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(20,7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exchanges");

            migrationBuilder.DropTable(
                name: "rates");
        }
    }
}
