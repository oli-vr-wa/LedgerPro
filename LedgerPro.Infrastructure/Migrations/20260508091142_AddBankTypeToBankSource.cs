using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankTypeToBankSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankType",
                table: "BankSources",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankType",
                table: "BankSources");
        }
    }
}
