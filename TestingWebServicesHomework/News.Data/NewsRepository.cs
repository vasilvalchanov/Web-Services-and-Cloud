using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Data
{
    using System.Data.Entity;

    using News.Data.Interfaces;
    using News.Models;

    public class NewsRepository : IRepository<News>
    {
        private NewsContext contex;

        public NewsRepository(NewsContext context)
        {
            this.contex = context;
        }

        public News Add(News entity)
        {
            var news = this.contex.News.Add(entity);
            return news;
        }

        public News Find(int id)
        {
            var news = this.contex.News.Find(id);
            return news;
        }

        public IQueryable<News> All()
        {
            var news = this.contex.News;
            return news;
        }

        public void Delete(News entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void Update(News entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void SaveChanges()
        {
            this.contex.SaveChanges();
        }

        private void ChangeState(News news, EntityState state)
        {
            var entry = this.contex.Entry(news);
            if (entry.State == EntityState.Detached)
            {
                this.contex.Set<News>().Attach(news);
            }

            entry.State = state;
        }
    }
}
