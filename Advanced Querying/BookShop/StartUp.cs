namespace BookShop
{
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
            RemoveBooks(db);
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            int intCommand;
            if (command.ToLower() == "minor")
            {
                intCommand = 0;
            }
            else if (command.ToLower() == "teen")
            {
                intCommand = 1;
            }
            else
            {
                intCommand = 2;
            }

            var books = context.Books
                .Where(b => (int) b.AgeRestriction == intCommand)
                .Select(b =>  b.Title)
                .ToList();

            StringBuilder sb = new();
            foreach (var book in books.OrderBy(b => b))
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 5000 && (int) b.EditionType == 2)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            StringBuilder sb = new();
            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            StringBuilder sb = new();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                        .Where(b => b.ReleaseDate.Value.Year != year)
                        .OrderBy(b => b.BookId)
                        .Select(b => b.Title)
                        .ToList();

            StringBuilder sb = new();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
            categories = categories.Select(c => c.ToLower()).ToList();
            var books = context.BooksCategories
                .Where(book => categories.Contains(book.Category.Name.ToLower()))
                .OrderBy(bc => bc.Book.Title)
                .Select(bc => bc.Book.Title)
                .ToList();

            StringBuilder sb = new();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            string[] yearSplitted = date.Split('-', StringSplitOptions.RemoveEmptyEntries);
            int day = Convert.ToInt32(yearSplitted[0]);
            int month = Convert.ToInt32(yearSplitted[1]);
            int year = Convert.ToInt32(yearSplitted[2]);
            DateTime givenDateTime = new DateTime(year, month, day);

            var allBooks = context.Books
                .Where(b => b.ReleaseDate < givenDateTime)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            StringBuilder sb = new();
            foreach (var book in allBooks)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType.ToString()} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            StringBuilder sb = new();

            foreach (var author in authors)
            {
                sb.AppendLine(author.FullName);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            StringBuilder sb = new();
            foreach (var book in books)
            {
                sb.AppendLine(book);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new 
                {
                    b.Title,
                    AuthorFullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToList();

            StringBuilder sb = new();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.AuthorFullName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Where(b => b.Title.Length > lengthCheck).Count();
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    Count = a.Books.Sum(b => b.Copies)
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var authour in authors.OrderByDescending(a => a.Count))
            {
                sb.AppendLine($"{authour.FullName} - {authour.Count}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .ToList()
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.CategoryName);

            StringBuilder sb = new StringBuilder();

            foreach (var bc in categories)
            {
                sb.AppendLine($"{bc.CategoryName} ${bc.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks.Select(cb => cb.Book)
                    .OrderByDescending(b => b.ReleaseDate).Take(3).ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories.OrderBy(c => c.CategoryName))
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(b => b.ReleaseDate.Value.Year < 2010).ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(b => b.Copies < 4200).ToList();
            context.Books.RemoveRange(books);
            int counter = books.Count;
            context.SaveChanges();
            return counter; 
        }
    }
}


