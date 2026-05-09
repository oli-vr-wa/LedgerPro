using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LedgerPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBankTransactionMatchDescriptionReferenceTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionTemplate",
                table: "BankTransactionMappings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceTemplate",
                table: "BankTransactionMappings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionTemplate",
                table: "BankTransactionMappings");

            migrationBuilder.DropColumn(
                name: "ReferenceTemplate",
                table: "BankTransactionMappings");
        }
    }
}
