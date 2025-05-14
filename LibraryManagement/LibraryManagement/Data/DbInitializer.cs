// DbInitializer.cs
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public static class DbInitializer
    {

        // DbInitializer.cs or can be added to your Program.cs
        public static async Task SeedBooks(LibraryDbContext context)
        {
            // Check if there are already books in the database
            if (await context.Books.AnyAsync())
                return;

            var books = new List<Book>
    {
        new Book
        {
            Title = "To Kill a Mockingbird",
            Author = "Harper Lee",
            Genre = "Fiction",
            PublishedYear = new DateTime(1960, 7, 11),
            Description = "The story of racial injustice and the loss of innocence in the American South during the Great Depression.",
            Quantity = 15
        },
        new Book
        {
            Title = "1984",
            Author = "George Orwell",
            Genre = "Dystopian",
            PublishedYear = new DateTime(1949, 6, 8),
            Description = "A dystopian novel set in a totalitarian society where critical thought is suppressed.",
            Quantity = 12
        },
        new Book
        {
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            Genre = "Classic",
            PublishedYear = new DateTime(1925, 4, 10),
            Description = "A novel that examines the dark side of the American Dream during the Roaring Twenties.",
            Quantity = 8
        },
        new Book
        {
            Title = "Harry Potter and the Philosopher's Stone",
            Author = "J.K. Rowling",
            Genre = "Fantasy",
            PublishedYear = new DateTime(1997, 6, 26),
            Description = "The first novel in the Harry Potter series that follows a young wizard's journey at Hogwarts School of Witchcraft and Wizardry.",
            Quantity = 20
        },
        new Book
        {
            Title = "The Hobbit",
            Author = "J.R.R. Tolkien",
            Genre = "Fantasy",
            PublishedYear = new DateTime(1937, 9, 21),
            Description = "A fantasy novel about the adventures of hobbit Bilbo Baggins, who embarks on a quest to help a group of dwarves reclaim their mountain home.",
            Quantity = 10
        },
        new Book
        {
            Title = "Pride and Prejudice",
            Author = "Jane Austen",
            Genre = "Romance",
            PublishedYear = new DateTime(1813, 1, 28),
            Description = "A romantic novel that follows the emotional development of Elizabeth Bennet, who learns about the repercussions of hasty judgments.",
            Quantity = 7
        },
        new Book
        {
            Title = "The Catcher in the Rye",
            Author = "J.D. Salinger",
            Genre = "Fiction",
            PublishedYear = new DateTime(1951, 7, 16),
            Description = "A novel that explores the themes of teenage angst, alienation, and identity through the eyes of Holden Caulfield.",
            Quantity = 9
        },
        new Book
        {
            Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
            Author = "Robert C. Martin",
            Genre = "Programming",
            PublishedYear = new DateTime(2008, 8, 1),
            Description = "A book about how to write clean, readable code that is easy to maintain and extend.",
            Quantity = 5
        },
        new Book
        {
            Title = "The Alchemist",
            Author = "Paulo Coelho",
            Genre = "Fiction",
            PublishedYear = new DateTime(1988, 1, 1),
            Description = "A philosophical novel about a young Andalusian shepherd who dreams of finding a worldly treasure.",
            Quantity = 11
        },
        new Book
        {
            Title = "Brave New World",
            Author = "Aldous Huxley",
            Genre = "Dystopian",
            PublishedYear = new DateTime(1932, 1, 1),
            Description = "A dystopian novel set in a futuristic society where humans are genetically bred and pharmaceutically conditioned to serve in a ruling order.",
            Quantity = 6
        },
        new Book
        {
            Title = "The Lord of the Rings",
            Author = "J.R.R. Tolkien",
            Genre = "Fantasy",
            PublishedYear = new DateTime(1954, 7, 29),
            Description = "An epic high-fantasy novel that follows the quest to destroy the One Ring, which was created by the Dark Lord Sauron.",
            Quantity = 14
        },
        new Book
        {
            Title = "Crime and Punishment",
            Author = "Fyodor Dostoevsky",
            Genre = "Psychological Fiction",
            PublishedYear = new DateTime(1866, 1, 1),
            Description = "A novel that explores the moral dilemmas of a poor ex-student who commits a murder to prove his own superiority.",
            Quantity = 4
        },
        new Book
        {
            Title = "The Little Prince",
            Author = "Antoine de Saint-Exupéry",
            Genre = "Children's Literature",
            PublishedYear = new DateTime(1943, 4, 6),
            Description = "A poetic tale about a young prince who visits various planets in space, including Earth.",
            Quantity = 17
        },
        new Book
        {
            Title = "Dune",
            Author = "Frank Herbert",
            Genre = "Science Fiction",
            PublishedYear = new DateTime(1965, 8, 1),
            Description = "A science fiction novel set in the distant future amidst a feudal interstellar society where noble houses control planetary fiefs.",
            Quantity = 8
        },
        new Book
        {
            Title = "The Hunger Games",
            Author = "Suzanne Collins",
            Genre = "Young Adult",
            PublishedYear = new DateTime(2008, 9, 14),
            Description = "A dystopian novel set in a post-apocalyptic society where young representatives from each district are chosen for a televised death match.",
            Quantity = 13
        },
        new Book
        {
            Title = "The Da Vinci Code",
            Author = "Dan Brown",
            Genre = "Mystery",
            PublishedYear = new DateTime(2003, 3, 18),
            Description = "A mystery thriller novel that follows symbologist Robert Langdon as he investigates a murder in the Louvre Museum.",
            Quantity = 9
        },
        new Book
        {
            Title = "The Kite Runner",
            Author = "Khaled Hosseini",
            Genre = "Historical Fiction",
            PublishedYear = new DateTime(2003, 5, 29),
            Description = "A novel about friendship, betrayal, and redemption set against the backdrop of Afghanistan's tumultuous history.",
            Quantity = 7
        },
        new Book
        {
            Title = "The Shining",
            Author = "Stephen King",
            Genre = "Horror",
            PublishedYear = new DateTime(1977, 1, 28),
            Description = "A horror novel about a family that becomes the caretakers of an isolated hotel with a supernatural presence.",
            Quantity = 6
        },
        new Book
        {
            Title = "The Art of War",
            Author = "Sun Tzu",
            Genre = "Philosophy",
            PublishedYear = new DateTime(500, 1, 1), // Approximate date
            Description = "An ancient Chinese military treatise that has influenced both Eastern and Western military thinking, business tactics, and beyond.",
            Quantity = 5
        },
        new Book
        {
            Title = "Sapiens: A Brief History of Humankind",
            Author = "Yuval Noah Harari",
            Genre = "Non-fiction",
            PublishedYear = new DateTime(2011, 1, 1),
            Description = "A book that explores the history and impact of Homo sapiens on the world.",
            Quantity = 10
        }
    };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<LibraryDbContext>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed users (if needed)
            // await SeedUsers(context);

            // Seed books
            await SeedBooks(context);
        }

        // Add the SeedBooks method here
    }
}