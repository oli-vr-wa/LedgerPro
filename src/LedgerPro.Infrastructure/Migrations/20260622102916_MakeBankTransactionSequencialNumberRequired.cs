using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeBankTransactionSequencialNumberRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FinancialYearSequencialNumber",
                table: "BankTransactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FinancialYearSequencialNumber",
                table: "BankTransactions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
