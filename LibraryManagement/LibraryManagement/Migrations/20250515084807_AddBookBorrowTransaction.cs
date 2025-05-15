using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddBookBorrowTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BorrowTransactions_BorrowTransactionId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BorrowTransactionId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BorrowTransactionId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BookBorrowTransactions",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: false),
                    BorrowTransactionId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookBorrowTransactions", x => new { x.BookId, x.BorrowTransactionId });
                    table.ForeignKey(
                        name: "FK_BookBorrowTransactions_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookBorrowTransactions_BorrowTransactions_BorrowTransactionId",
                        column: x => x.BorrowTransactionId,
                        principalTable: "BorrowTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrowTransactions_BorrowTransactionId",
                table: "BookBorrowTransactions",
                column: "BorrowTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookBorrowTransactions");

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
    }
}
