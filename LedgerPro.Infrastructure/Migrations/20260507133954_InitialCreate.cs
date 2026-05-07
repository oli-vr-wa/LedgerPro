using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", nullable: false),
                    BankName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Classification = table.Column<int>(type: "INTEGER", nullable: false),
                    IsClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneralLedgerAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AccountType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralLedgerAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatementImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    ImportDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FileHash = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionCount = table.Column<int>(type: "INTEGER", nullable: false),
                    BankSourceId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatementImports_BankSources_BankSourceId",
                        column: x => x.BankSourceId,
                        principalTable: "BankSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankTransactionMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SearchTerm = table.Column<string>(type: "TEXT", nullable: false),
                    MatchStrategy = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetGeneralLedgerAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransactionMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransactionMappings_GeneralLedgerAccounts_TargetGeneralLedgerAccountId",
                        column: x => x.TargetGeneralLedgerAccountId,
                        principalTable: "GeneralLedgerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    TransactionType = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BankSourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StatementImportId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransactions_BankSources_BankSourceId",
                        column: x => x.BankSourceId,
                        principalTable: "BankSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankTransactions_StatementImports_StatementImportId",
                        column: x => x.StatementImportId,
                        principalTable: "StatementImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneralLedgerItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reference = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    Side = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneralLedgerAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    BankTransactionId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralLedgerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralLedgerItems_BankTransactions_BankTransactionId",
                        column: x => x.BankTransactionId,
                        principalTable: "BankTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GeneralLedgerItems_GeneralLedgerAccounts_GeneralLedgerAccountId",
                        column: x => x.GeneralLedgerAccountId,
                        principalTable: "GeneralLedgerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactionMappings_TargetGeneralLedgerAccountId",
                table: "BankTransactionMappings",
                column: "TargetGeneralLedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_BankSourceId",
                table: "BankTransactions",
                column: "BankSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_StatementImportId",
                table: "BankTransactions",
                column: "StatementImportId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLedgerItems_BankTransactionId",
                table: "GeneralLedgerItems",
                column: "BankTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLedgerItems_GeneralLedgerAccountId",
                table: "GeneralLedgerItems",
                column: "GeneralLedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementImports_BankSourceId",
                table: "StatementImports",
                column: "BankSourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankTransactionMappings");

            migrationBuilder.DropTable(
                name: "FinancialPeriods");

            migrationBuilder.DropTable(
                name: "GeneralLedgerItems");

            migrationBuilder.DropTable(
                name: "BankTransactions");

            migrationBuilder.DropTable(
                name: "GeneralLedgerAccounts");

            migrationBuilder.DropTable(
                name: "StatementImports");

            migrationBuilder.DropTable(
                name: "BankSources");
        }
    }
}
