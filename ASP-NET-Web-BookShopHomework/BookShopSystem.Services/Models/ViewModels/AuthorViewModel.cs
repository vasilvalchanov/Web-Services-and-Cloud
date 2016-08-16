using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Models
{
    using BookShopSystem.Models;
    using BookShopSystem.Services.Models.ViewModels;

    public class AuthorViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<string> BookTitles { get; set; }

        public static AuthorViewModel Create(Author author)
        {
            var authorModel = new AuthorViewModel()
                                  {
                                      Id = author.Id,
                                      FirstName = author.FirstName,
                                      LastName = author.LastName,
                                      BookTitles = new List<string>()
                                  };
            AddBookTitles(authorModel, author.Books);

            return authorModel;

        }

        private static void AddBookTitles(AuthorViewModel authorViewModel, ICollection<Book> books)
        {
            foreach (var book in books)
            {
                authorViewModel.BookTitles.Add(book.Title);
            }
        }
    }
}