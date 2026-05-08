using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralLedgerItemIsReconciled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReconciled",
                table: "GeneralLedgerItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReconciled",
                table: "GeneralLedgerItems");
        }
    }
}
