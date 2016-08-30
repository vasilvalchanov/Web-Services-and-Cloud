using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace News.Services.Controllers
{
    using System.Drawing.Text;
    using System.Net.Http;
    using System.Web.Http;

    using News.Data;
    using News.Data.Interfaces;
    using News.Models;
    using News.Services.Models;

    [RoutePrefix("api/news")]
    public class NewsController : ApiController
    {
        private IRepository<News> repository;

        private string authToken;

        public NewsController(IRepository<News> repository)
        {
            this.repository = repository;
        }

        public NewsController() : this(new NewsRepository(new NewsContext()))
        {
        }

        [HttpGet] // If I return HttpActionResult by testing with CollectionAssert.AreEquivalent
        public IQueryable<News> GetNews() // test fails. Test passes only with IQuerable
        {
            var news = this.repository.All().OrderByDescending(n => n.PublishDate);

            return news;
        }

        [HttpPost]
        public IHttpActionResult CreateNews([FromBody]NewsBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.repository.All().Any(n => n.Title == model.Title))
            {
                return this.BadRequest("The news with such title already exists");
            }

            var news = new News()
            {
                Title = model.Title,
                Content = model.Content,
                PublishDate = DateTime.Now
            };

            var addedNews = this.repository.Add(news);
            this.repository.SaveChanges();

            return this.Created("DefaultApi", new { Title = addedNews.Title, Content = addedNews.Content, PublishDate = addedNews.PublishDate });
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateNewsById([FromUri]int id, [FromBody]NewsBindingModel model)
        {
            var news = this.repository.Find(id);

            if (news == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.repository.All().Any(n => n.Title == model.Title && n.Id != news.Id))
            {
                return this.BadRequest("The news with such title already exists");
            }

            news.Title = model.Title;
            news.Content = model.Content;
            news.PublishDate = news.PublishDate;

            this.repository.Update(news);
            this.repository.SaveChanges();

            return this.Ok(news);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteNews(int id)
        {
            var news = this.repository.Find(id);

            if (news == null)
            {
                return this.BadRequest();
            }

            this.repository.Delete(news);
            this.repository.SaveChanges();

            return this.Ok(string.Format("The news with id {0} was deleted", id));
        }
    }
}