using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<BookBorrowTransaction> BookBorrowTransactions { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<BookBorrowTransaction>()
    .HasKey(bb => new { bb.BookId, bb.BorrowTransactionId });


            modelBuilder.Entity<BookBorrowTransaction>()
                .HasOne(bb => bb.BorrowTransaction)
                .WithMany(bt => bt.BookBorrowTransactions)
                .HasForeignKey(bb => bb.BorrowTransactionId);


        }


    }
}