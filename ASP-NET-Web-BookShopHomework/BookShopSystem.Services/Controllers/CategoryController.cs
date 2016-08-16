using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Controllers
{
    using System.Web.Http;
    using System.Web.OData;

    using BookShopSystem.Models;
    using BookShopSystem.Services.Models.BindingModels;

    [RoutePrefix("api/categories")]
    public class CategoryController : BaseController
    {
        [HttpGet]
        [Route("")]
        [EnableQuery]
        public IHttpActionResult GetCategories()
        {
            var categories = this.Context.Categories
                .Select(c => new
                                 {
                                     Id = c.Id,
                                     Name = c.Name
                                 });

            if (!categories.Any())
            {
                return this.NotFound();
            }

            return this.Ok(categories);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetCategoryById(int id)
        {
            var category = this.Context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return this.BadRequest("There is not category with such id");
            }

            var result = new { Id = category.Id, Name = category.Name };

            return this.Ok(result);
        }


        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditCategoryName(int id, [FromBody]CategoryBindingModel model)
        {
            var category = this.Context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return this.BadRequest("There is not category with such id");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.Context.Categories.Any(c => c.Name == model.Name))
            {
                return this.BadRequest("The category name already exists");
            }

            category.Name = model.Name;
            this.Context.SaveChanges();

            return this.Ok("The category name was edited.");
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteCategory(int id)
        {
            var category = this.Context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return this.BadRequest("There is not category with such id");
            }

            this.Context.Categories.Remove(category);
            this.Context.SaveChanges();

            return this.Ok("The category was deleted.");
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult AddNewCategory([FromBody]CategoryBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var category = new Category()
                               {
                                   Name = model.Name
                               };

            this.Context.Categories.Add(category);
            this.Context.SaveChanges();

            return this.Ok("The category was added successfully");
        }
    }
}