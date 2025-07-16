using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillFlow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Institution = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BillTags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillTags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBalanceHistory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    StartingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EndingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeposits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDebits = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBalanceHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MonthlyBalanceHistory_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BillTagID = table.Column<int>(type: "INTEGER", nullable: true),
                    IsMonthy = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPayedOff = table.Column<bool>(type: "INTEGER", nullable: false),
                    Frequency = table.Column<string>(type: "TEXT", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsFixedAmount = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastGeneratedMonth = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bills_BillTags_BillTagID",
                        column: x => x.BillTagID,
                        principalTable: "BillTags",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BillInstances",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillID = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillInstances", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BillInstances_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BillID = table.Column<int>(type: "INTEGER", nullable: false),
                    BillInstanceID = table.Column<int>(type: "INTEGER", nullable: true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: true),
                    SecondaryAccountID = table.Column<int>(type: "INTEGER", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SecondaryAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payments_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Payments_Accounts_SecondaryAccountID",
                        column: x => x.SecondaryAccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Payments_BillInstances_BillInstanceID",
                        column: x => x.BillInstanceID,
                        principalTable: "BillInstances",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Payments_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentID = table.Column<int>(type: "INTEGER", nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillInstances_BillID_Year_Month",
                table: "BillInstances",
                columns: new[] { "BillID", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_BillTagID",
                table: "Bills",
                column: "BillTagID");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBalanceHistory_AccountID_Year_Month",
                table: "MonthlyBalanceHistory",
                columns: new[] { "AccountID", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AccountID",
                table: "Payments",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillID",
                table: "Payments",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillInstanceID",
                table: "Payments",
                column: "BillInstanceID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SecondaryAccountID",
                table: "Payments",
                column: "SecondaryAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountID",
                table: "Transactions",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentID",
                table: "Transactions",
                column: "PaymentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyBalanceHistory");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BillInstances");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "BillTags");
        }
    }
}
