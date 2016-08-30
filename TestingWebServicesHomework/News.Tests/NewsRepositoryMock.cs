using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Tests
{
    using News.Data.Interfaces;
    using News.Models;

    public class NewsRepositoryMock : IRepository<News>
    {
 
        public NewsRepositoryMock()
        {
            this.News = new List<News>();
        }

        public IList<News> News { get; set; }

        public bool IsSaveCalled { get; set; }

        public News Add(News news)
        {
            this.GenerateEntityId(news);
            this.News.Add(news);

            return news;
        }

        public News Find(int id)
        {
            var news = this.News.FirstOrDefault(n => n.Id == id);

            return news;
        }

        public IQueryable<News> All()
        {
            return this.News.AsQueryable();
        }

        public void Delete(News news)
        {
            this.News.Remove(news);
        }

        public void Update(News news)
        {
            this.News.Remove(news);
            this.News.Add(news);
        }

        public void SaveChanges()
        {
            this.IsSaveCalled = true;
        }

        private void GenerateEntityId(News news) // We have to do this, cause we don't work
        {                                        // with DB and the Ids don't generate automatic
            if (!this.News.Any())                // and the Id is required for the entity
            {
                news.Id = 1;
            }
            else
            {
                var lastId = this.News.LastOrDefault().Id;
                news.Id = lastId + 1;
            }
        }
    }
}
