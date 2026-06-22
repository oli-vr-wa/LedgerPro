using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BankTransactionFinancialYearSequencialNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinancialYearSequencialNumber",
                table: "BankTransactions",
                type: "INTEGER",
                nullable: true);

            // Initialize existing transactions with financial year sequence numbers.
            migrationBuilder.Sql(@"
                WITH RankedTransactions AS (
                    SELECT 
                        Id,
                        BankSourceId,
                        TransactionDate,
                        ROW_NUMBER() OVER (PARTITION BY BankSourceId, strftime('%Y', TransactionDate) ORDER BY TransactionDate) AS FinancialYearSeq
                    FROM BankTransactions
                )
                UPDATE BankTransactions
                SET FinancialYearSequencialNumber = (
                    SELECT FinancialYearSeq 
                    FROM RankedTransactions 
                    WHERE RankedTransactions.Id = BankTransactions.Id
                    );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialYearSequencialNumber",
                table: "BankTransactions");
        }
    }
}
