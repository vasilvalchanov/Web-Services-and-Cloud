namespace BookShopSystem.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using BookShopSystem.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<BookShopSystem.Data.BookShopSystemContext>
    {
        private string path;

        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BookShopSystem.Data.BookShopSystemContext context)
        {
            path = HttpContext.Current.Server.MapPath("~/App_Data/");

            if (!context.Authors.Any())
            {
                this.SeedAuthors(context);
            }

            if (!context.Books.Any())
            {
                this.SeedBooks(context);
            }

            if (!context.Categories.Any())
            {
                this.SeedCategories(context);
            }

        }

        private void SeedAuthors(BookShopSystemContext bookShopContext)
        {
            using (var streamReader = new StreamReader(path + @"authors.txt"))
            {
                var line = streamReader.ReadLine();
                line = streamReader.ReadLine();

                while (line != null)
                {
                    var data = line.Split(' ');
                    var firstName = data[0];
                    var lastName = data[1];

                    bookShopContext.Authors.Add(new Author() { FirstName = firstName, LastName = lastName });
                    line = streamReader.ReadLine();
                }

                bookShopContext.SaveChanges();
            }
        }

        private void SeedBooks(BookShopSystemContext bookShopContext)
        {

            using (var reader = new StreamReader(path + @"books.txt"))
            {
                var random = new Random();
                var line = reader.ReadLine();
                line = reader.ReadLine();
                var authors = bookShopContext.Authors.Select(a => a.Id).ToList();

                while (line != null)
                {
                    var data = line.Split(new[] { ' ' }, 6);
                    var authorIndex = random.Next(0, authors.Count);
                    var authorId = authors[authorIndex];
                    var edition = (EditionType)int.Parse(data[0]);
                    var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                    var copies = int.Parse(data[2]);
                    var price = decimal.Parse(data[3]);
                    var ageRestriction = (AgeRestriction)int.Parse(data[4]);
                    var title = data[5];

                    bookShopContext.Books.Add(new Book()
                    {
                        Author = bookShopContext.Authors.FirstOrDefault(a => a.Id == authorId),
                        EditionType = edition,
                        ReleasDate = releaseDate,
                        Copies = copies,
                        Price = price,
                        AgeRestriction = ageRestriction,
                        Title = title
                    });

                    line = reader.ReadLine();
                }

                bookShopContext.SaveChanges();
            }

        }

        private void SeedCategories(BookShopSystemContext bookShopContext)
        {
            using (var streamReader = new StreamReader(path + @"categories.txt"))
            {
                var random = new Random();
                var line = streamReader.ReadLine();
                var categories = new List<string>();


                while (line != null)
                {
                    categories.Add(line);
                    bookShopContext.Categories.Add(new Category() { Name = line });
                    line = streamReader.ReadLine();
                }

                bookShopContext.SaveChanges();

                foreach (var book in bookShopContext.Books)
                {
                    var arbitraryId = random.Next(0, categories.Count);
                    var catName = categories[arbitraryId];
                    var category =
                        bookShopContext.Categories.FirstOrDefault(c => c.Name == catName);
                    category.Books.Add(book);
                    book.Categories.Add(category);
                }

                bookShopContext.SaveChanges();
            }
        }
    }
}
