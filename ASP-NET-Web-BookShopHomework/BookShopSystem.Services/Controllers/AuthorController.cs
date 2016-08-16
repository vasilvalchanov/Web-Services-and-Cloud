using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Controllers
{
    using System.Web.Http;
    using System.Web.OData;

    using BookShopSystem.Models;
    using BookShopSystem.Services.Models;
    using BookShopSystem.Services.Models.BindingModels;
    using BookShopSystem.Services.Models.ViewModels;

    [RoutePrefix("api/authors")]
    public class AuthorController : BaseController
    {
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetAuthor(int id)
        {
            var author = this.Context.Authors.Find(id);

            if (author == null)
            {
                return this.BadRequest("There is not author with such id.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var result = AuthorViewModel.Create(author);
            return this.Ok(result);
        }


        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateAuthor(CreateAuthorBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.Context.Authors.Any(a => a.FirstName == model.FirstName && a.LastName == model.LastName))
            {
                return this.BadRequest("Author with same name already exists.");
            }

            var newAuthor = new Author()
                                {
                                    FirstName = model.FirstName,
                                    LastName = model.LastName
                                };

            this.Context.Authors.Add(newAuthor);
            this.Context.SaveChanges();

            return this.Ok(newAuthor);

        }

        [HttpGet]
        [Route("{id}/books")]
        [EnableQuery]
        public IHttpActionResult GetBooksByAuthor(int id)
        {
            var author = this.Context.Authors.Find(id);

            if (author == null)
            {
                return this.BadRequest("There is not author with such id.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var books = author.Books.Select(BookViewModel.Create);

            return this.Ok(books);
        } 
    }
}