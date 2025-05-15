using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBorrowTransactionIdFromBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowedBooks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "BorrowTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "BorrowTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorrowTransactionId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_BorrowTransactionId",
                table: "Books",
                column: "BorrowTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BorrowTransactions_BorrowTransactionId",
                table: "Books",
                column: "BorrowTransactionId",
                principalTable: "BorrowTransactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BorrowTransactions_BorrowTransactionId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BorrowTransactionId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "BorrowTransactions");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "BorrowTransactions");

            migrationBuilder.DropColumn(
                name: "BorrowTransactionId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BorrowedBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    BorrowTransactionId = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowedBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowedBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowedBooks_BorrowTransactions_BorrowTransactionId",
                        column: x => x.BorrowTransactionId,
                        principalTable: "BorrowTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_BookId",
                table: "BorrowedBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_BorrowTransactionId",
                table: "BorrowedBooks",
                column: "BorrowTransactionId");
        }
    }
}
