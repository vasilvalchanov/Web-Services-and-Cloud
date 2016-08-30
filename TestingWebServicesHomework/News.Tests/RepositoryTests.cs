using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace News.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;

    using EntityFramework.Extensions;

    using News.Data;
    using News.Data.Interfaces;
    using News.Models;
    using News.Services.Controllers;

    [TestClass]
    public class RepositoryTests
    {
        private IRepository<News> repo;

        [TestInitialize]
        public void TestInitialize()
        {
            this.repo = new NewsRepository(new NewsContext());
            this.CleanDatabase();
        }

        [TestMethod]
        public void Get_All_News_Should_Return_All_News_Correctly()
        {
            var newsList = new List<News>()
            {
                new News()
                {
                    Title = "Title1",
                    Content = "Content1",
                    PublishDate = DateTime.Now
                },
                new News()
                {
                    Title = "Title2",
                    Content = "Content2",
                    PublishDate = DateTime.Now
                },
                new News()
                {
                    Title = "Title3",
                    Content = "Content3",
                    PublishDate = DateTime.Now
                },
            };


            foreach (var news in newsList)
            {
                this.repo.Add(news);
            }

            this.repo.SaveChanges();

            var resultNews = this.repo.All().ToList();


            Assert.IsTrue(resultNews.Count() == 3);
            CollectionAssert.AreEquivalent(newsList, resultNews);

        }

        [TestMethod]
        public void Add_News_With_Valid_Data_Should_Return_Item()
        {
            var newsToAdd = new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now };
            var resultNews = this.repo.Add(newsToAdd);
            this.repo.SaveChanges();

            var newsFromDb = this.repo.Find(newsToAdd.Id);

            Assert.AreEqual(newsFromDb.Title, resultNews.Title);
            Assert.AreEqual(newsFromDb.Content, resultNews.Content);
            Assert.AreEqual(newsFromDb.PublishDate, resultNews.PublishDate);
            Assert.IsTrue(this.repo.All().Count() == 1);
            Assert.AreNotEqual(0, newsFromDb.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Add_News_With_InValid_Data_Should_Throw()
        {
            var newsToAdd = new News() { Content = "Content1", PublishDate = DateTime.Now };
            this.repo.Add(newsToAdd);
            this.repo.SaveChanges();
        }

        [TestMethod]
        public void Update_Existing_News_Should_Edit_It_Correctly()
        {
            var newsToAdd = new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now };
            this.repo.Add(newsToAdd);
            this.repo.SaveChanges();

            var newsToUpdate = this.repo.Find(newsToAdd.Id);
            newsToUpdate.Content = "Baba Gicka olimpiec";
            this.repo.Update(newsToUpdate);
            this.repo.SaveChanges();

            var updatedNewsFromDb = this.repo.Find(newsToUpdate.Id);

            Assert.AreEqual("Baba Gicka olimpiec", updatedNewsFromDb.Content);
            Assert.AreEqual(newsToUpdate.Title, updatedNewsFromDb.Title);
            Assert.AreEqual(newsToUpdate.Id, updatedNewsFromDb.Id);
            Assert.AreEqual(newsToUpdate.PublishDate, updatedNewsFromDb.PublishDate);
           
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void Update_Existing_News_With_Invalid_Data_Should_Throw()
        {
            var newsToAdd = new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now };
            this.repo.Add(newsToAdd);
            this.repo.SaveChanges();

            var newsToUpdate = this.repo.Find(newsToAdd.Id);
            newsToUpdate.Content = null;
            this.repo.Update(newsToUpdate);
            this.repo.SaveChanges();

        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void Update_NonExisting_News_Should_Throw()
        {
            var newsToAdd = new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now };
            this.repo.Add(newsToAdd);


            newsToAdd.Content = "Updated content";
            this.repo.Update(newsToAdd);
            this.repo.SaveChanges();

        }

        [TestMethod]
        public void Delete_Existing_News_Should_Delete_It()
        {
            var newsToAdd = new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now };
            this.repo.Add(newsToAdd);
            this.repo.SaveChanges();

            this.repo.Delete(newsToAdd);
            this.repo.SaveChanges();

            Assert.IsTrue(!this.repo.All().Any());
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateConcurrencyException))]
        public void Delete_NonExisting_News_Should_Throw()
        {
            var newsToAdd = new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now };
            this.repo.Add(newsToAdd);

            this.repo.Delete(newsToAdd);
            this.repo.SaveChanges();
        }

        private void CleanDatabase()
        {
            // Clean all data in all database tables
            var context = new NewsContext();
            context.News.Delete();
            context.SaveChanges();
        }
    }
}
