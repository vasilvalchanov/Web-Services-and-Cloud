using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Controllers
{
    using System.Data.Entity.Validation;
    using System.Text;
    using System.Web.Http;
    using System.Web.OData;

    using BookShopSystem.Models;
    using BookShopSystem.Services.Models.BindingModels;
    using BookShopSystem.Services.Models.ViewModels;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/books")]
    public class BookController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBook(int id)
        {
            var book = this.Context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.BadRequest("There is not a book with this id");
            }

            var result = BookViewModel.Create(book);
            return this.Ok(result);
        }

        [HttpGet]
        [Route("")]  // this empty string will create the given route /api/books?search={word}
        [EnableQuery]
        public IHttpActionResult GetBooksByKeyWord(string search)
        {
            var books = this.Context.Books.Where(b => b.Title.Contains(search)).OrderBy(b => b.Title).Take(10);

            if (!books.Any())
            {
                return this.NotFound();
            }

            var result = books.Select(b => new { Id = b.Id, Title = b.Title });

            return this.Ok(result);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditBook(int id, EditBookBindingModel model)
        {
            var book = this.Context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.BadRequest("There is not book with such id");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            book.Title = model.Title ?? book.Title;
            book.Description = model.Description ?? book.Description;
            book.Price = model.Price ?? book.Price;
            book.Copies = model.Copies ?? book.Copies;
            book.EditionType = model.EditionType ?? book.EditionType;
            book.AgeRestriction = model.AgeRestriction ?? book.AgeRestriction;
            book.ReleasDate = model.ReliesDate ?? book.ReleasDate;
            book.AuthorId = model.AuthorId ?? book.AuthorId;

            this.Context.SaveChanges();

            return this.Ok("The book was edit successfully");


        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteBook(int id)
        {
            var book = this.Context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return this.BadRequest("There is not book with such id");
            }

            this.Context.Books.Remove(book);
            this.Context.SaveChanges();

            return this.Ok("The book was deleted.");
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult AddNewBook(AddBookBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.Context.Books.Any(b => b.Title == model.Title))
            {
                return this.BadRequest("The book with this title already exists.");
            }

            var categories = model.Categories.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var category in categories)
            {
                if (!this.Context.Categories.Any(c => c.Name == category))
                {
                    return this.BadRequest(string.Format("Category {0} is invalid", category));
                }
            }

            var book = new Book()
                           {
                               Title = model.Title,
                               Description = model.Description ?? "No Description",
                               Price = model.Price,
                               Copies = model.Copies,
                               EditionType = model.EditionType,
                               AgeRestriction = model.AgeRestriction,
                               ReleasDate = model.ReliesDate,
                               Categories = new List<Category>()
                           };

            foreach (var category in categories)
            {
                var categoryToAdd = this.Context.Categories.FirstOrDefault(c => c.Name == category);
                book.Categories.Add(categoryToAdd);
            }

            this.Context.Books.Add(book);
            this.Context.SaveChanges();

            return this.Ok("The book was added successfully");
        }

        [HttpPut]
        [Route("buy/{id}")]
        public IHttpActionResult BuyBook(int id)
        {
            //var book = this.Context.Books.FirstOrDefault(b => b.Id == id);

            //if (book == null)
            //{
            //    return this.BadRequest("There is not book with such id");
            //}

            //if (book.Copies == 0)
            //{
            //    return this.BadRequest("There are not available copies of this book");
            //}

            //var currentUser = this.User.Identity.GetUserId();

            //if (currentUser == null)
            //{
            //    return this.BadRequest("There is not logged in user.");
            //}

            //var user = this.Context.Users.FirstOrDefault(u => u.Id == currentUser);

            //var purchase = new Purchase()
            //                   {
            //                       User = user,
            //                       Book = book,
            //                       Price = book.Price,
            //                       DateOfPurchase = DateTime.Now,
            //                       IsRecalled = false
            //                   };

            //book.Copies--;
            //this.Context.Purchases.Add(purchase);
            //this.Context.SaveChanges();

            //return this.Ok("Your order was successfull");

            try
            {
                var book = this.Context.Books.FirstOrDefault(b => b.Id == id);

                if (book == null)
                {
                    return this.BadRequest("There is not book with such id");
                }

                if (book.Copies == 0)
                {
                    return this.BadRequest("There are not available copies of this book");
                }

                var currentUser = this.User.Identity.GetUserId();

                if (currentUser == null)
                {
                    return this.BadRequest("There is not logged in user.");
                }

                var user = this.Context.Users.FirstOrDefault(u => u.Id == currentUser);

                var purchase = new Purchase()
                {
                    User = user,
                    Book = book,
                    Price = book.Price,
                    DateOfPurchase = DateTime.Now,
                    IsRecalled = false
                };

                book.Copies--;
                this.Context.Purchases.Add(purchase);
                this.Context.SaveChanges();

                return this.Ok("Your order was successfull");
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    sb.AppendLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        sb.AppendLine();
                    }
                }
                throw;
            }
        }

        [HttpPut]
        [Route("recall/{id}")]
        public IHttpActionResult RecallBook(int id)
        {

            try
            {
                var purchase = this.Context.Purchases.FirstOrDefault(p => p.Id == id);

                if (purchase == null)
                {
                    return this.BadRequest("There is not purchase with such id");
                }

                var elapsedTimeInDays = (DateTime.Now - purchase.DateOfPurchase).TotalDays;

                if (elapsedTimeInDays > 30)
                {
                    return this.BadRequest("More then 30 days have passed since the purchase date");
                }

                purchase.IsRecalled = true;
                var book = this.Context.Books.FirstOrDefault(b => b.Id == purchase.Book.Id);
                book.Copies++;
                this.Context.SaveChanges();

                return this.Ok("The purchase was recalled");
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    sb.AppendLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        sb.AppendLine();
                    }
                }
                throw;
            }
        }
    }
}