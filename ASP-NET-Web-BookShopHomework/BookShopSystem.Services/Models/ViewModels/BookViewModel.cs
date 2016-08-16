using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Models.ViewModels
{
    using BookShopSystem.Models;

    using Microsoft.Ajax.Utilities;

    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public EditionType EditionType { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public decimal Price { get; set; }

        public int Copies { get; set; }

        public DateTime? ReliesDate { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public ICollection<string> Categories { get; set; }

        public ICollection<string> RelatedBooks { get; set; }

        public static BookViewModel Create(Book book)
        {
            var bookModel = new BookViewModel()
                                {
                                    Id = book.Id,
                                    Title = book.Title,
                                    Description = book.Description,
                                    EditionType = book.EditionType,
                                    AgeRestriction = book.AgeRestriction,
                                    Price = book.Price,
                                    Copies = book.Copies,
                                    ReliesDate = book.ReleasDate ?? book.ReleasDate.Value,
                                    AuthorId = book.AuthorId ?? book.AuthorId.Value,
                                    AuthorName = book.Author.FirstName + " " + book.Author.LastName,
                                    Categories = new List<string>(),
                                    RelatedBooks = new List<string>()
                                };

            AddCategories(bookModel, book.Categories);
            AddRelatedBooks(bookModel, book.RelatedBooks);

            return bookModel;
        }

        private static void AddCategories(BookViewModel model, ICollection<Category> categories)
        {
            foreach (var category in categories)
            {
                model.Categories.Add(category.Name);
            }
        }

        private static void AddRelatedBooks(BookViewModel model, ICollection<Book> relatedBooks)
        {
            foreach (var book in relatedBooks)
            {
                model.RelatedBooks.Add(book.Title);
            }
        }

    }
}