using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace News.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using EntityFramework.Extensions;

    using Microsoft.Owin.Builder;
    using Microsoft.Owin.Testing;

    using News.Data;
    using News.Data.Interfaces;
    using News.Models;
    using News.Services;
    using News.Services.Controllers;
    using News.Services.Models;

    using Owin;

    [TestClass]
    public class NewsControllerIntegrationTests
    {
        private static TestServer httpTestServer;

        private static HttpClient httpClient;

        private IRepository<News> repo;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            httpTestServer = TestServer.Create(
                appBuilder =>
                    {
                        var config = new HttpConfiguration();
                        WebApiConfig.Register(config);
                        var startup = new Startup();

                        startup.Configuration(appBuilder);
                        appBuilder.UseWebApi(config);
                    });

            httpClient = httpTestServer.HttpClient;
        }


        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }
        }

        [TestInitialize]
        public void TestInit()
        {
            var context = new NewsContext();
            this.repo = new NewsRepository(context);
            this.CleanDatabase();
        }

        [TestMethod]
        public void Get_All_News_Should_Return_News_Correctly()
        {

            var news = new List<News>()
                           {
                               new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now },
                               new News() { Title = "Title2", Content = "Content2", PublishDate = DateTime.Now },
                               new News() { Title = "Title3", Content = "Content3", PublishDate = DateTime.Now },
                               new News() { Title = "Title4", Content = "Content4", PublishDate = DateTime.Now }
                           };

            foreach (var newsItem in news)
            {
                this.repo.Add(newsItem);
            }

            this.repo.SaveChanges();

            var response = httpClient.GetAsync("api/news").Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(response.Content.Headers.ContentType.MediaType, "application/json");

            var newsFromResponse = response.Content.ReadAsAsync<List<News>>().Result;
            var newsFromDb = this.repo.All().ToList();

            Assert.AreEqual(newsFromDb.Count, newsFromResponse.Count);

            // Assert the bugs in the DB are the same as the bugs returned from the service
            // if we check them with Collection.Assert, the test will fail
            for (int i = 0; i < newsFromDb.Count; i++)
            {
                Assert.AreEqual(newsFromResponse[i].Id, newsFromDb[i].Id);
                Assert.AreEqual(newsFromResponse[i].Title, newsFromDb[i].Title);
                Assert.AreEqual(newsFromResponse[i].Content, newsFromDb[i].Content);
                Assert.AreEqual(newsFromResponse[i].PublishDate.ToString(), newsFromDb[i].PublishDate.ToString());
            }
        }

        [TestMethod]
        public void Create_News_With_Valid_Data_Should_Return_201Created_And_The_News()
        {
            var newsModel = new NewsBindingModel()
            {
                    Title = "Title1", Content = "Content1", PublishedDate = DateTime.Now 
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", newsModel.Title),
                new KeyValuePair<string, string>("Content", newsModel.Content),
                new KeyValuePair<string, string>("PublishDate", newsModel.PublishedDate.ToString()),     
            });

            var response = httpClient.PostAsync("api/news", content).Result;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsTrue(this.repo.All().Count() == 1);

            var newsFromResponse = response.Content.ReadAsAsync<News>().Result;
            var newsFromDb = this.repo.All().FirstOrDefault(n => n.Title == newsFromResponse.Title);

            Assert.AreEqual(newsFromResponse.Title, newsFromDb.Title);
            Assert.AreEqual(newsFromResponse.Content, newsFromDb.Content);
            Assert.AreEqual(newsFromResponse.PublishDate.ToString(), newsFromDb.PublishDate.ToString());
        }

        [TestMethod]
        public void Create_News_With_Incorect_Data_Should_Return_400BadRequest()
        {
            var newsModel = new NewsBindingModel()
            {
                Title = "Title1",
                Content = null,
                PublishedDate = DateTime.Now
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Title", newsModel.Title),
                new KeyValuePair<string, string>("Content", newsModel.Content),
                new KeyValuePair<string, string>("PublishDate", newsModel.PublishedDate.ToString()),
            });

            var response = httpClient.PostAsync("api/news", content).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(this.repo.All().Count() == 0);
        }

        [TestMethod]
        public void Edit_News_With_Valid_Data_Should_Return_200OK_And_Edited_News()
        {
            var news = new News()
                           {
                               Title = "Title1",
                               Content = "Content1",
                               PublishDate = DateTime.Now
                           };

            this.repo.Add(news);
            this.repo.SaveChanges();

            var newsToEditId = this.repo.All().FirstOrDefault(n => n.Title == news.Title).Id;

            var modelNews = new NewsBindingModel() { Title = "Edited News", Content = "Baba Gicka"};

            var content = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("Title", modelNews.Title),
                new KeyValuePair<string, string>("Content", modelNews.Content),
                new KeyValuePair<string, string>("PublishDate", modelNews.PublishedDate.ToString())
            });


            var response = httpClient.PutAsync("api/news/" + newsToEditId, content).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var context = new NewsContext();
            var newsFromDb = context.News.Find(newsToEditId);

            var editedNewsFromResponse = response.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(editedNewsFromResponse.Id, newsFromDb.Id);
            Assert.AreEqual(editedNewsFromResponse.Title, newsFromDb.Title);
            Assert.AreEqual(editedNewsFromResponse.Content, newsFromDb.Content);
            Assert.AreEqual(editedNewsFromResponse.PublishDate.ToString(), newsFromDb.PublishDate.ToString());
        }

        [TestMethod]
        public void Edit_News_With_Incorrect_Data_Should_Return_400BadRequest()
        {
            var news = new News()
            {
                Title = "Title1",
                Content = "Content1",
                PublishDate = DateTime.Now
            };

            this.repo.Add(news);
            this.repo.SaveChanges();

            var newsToEditId = this.repo.All().FirstOrDefault(n => n.Title == news.Title).Id;

            var modelNews = new NewsBindingModel() { Title = null, Content = "Baba Gicka" };

            var content = new FormUrlEncodedContent(new[]
           {
                new KeyValuePair<string, string>("Title", modelNews.Title),
                new KeyValuePair<string, string>("Content", modelNews.Content),
                new KeyValuePair<string, string>("PublishDate", modelNews.PublishedDate.ToString())
            });

            var resposne = httpClient.PutAsync("api/news/" + newsToEditId, content).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, resposne.StatusCode);
        }

        [TestMethod]
        public void Delete_News_With_Correct_Data_Should_Return_200OK_And_Delete_It()
        {
            var news = new List<News>()
                           {
                               new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now },
                               new News() { Title = "Title2", Content = "Content2", PublishDate = DateTime.Now },
                               new News() { Title = "Title3", Content = "Content3", PublishDate = DateTime.Now },
                               new News() { Title = "Title4", Content = "Content4", PublishDate = DateTime.Now }
                           };

            foreach (var newsItem in news)
            {
                this.repo.Add(newsItem);
            }

            this.repo.SaveChanges();

            var newsToDelete = this.repo.All().FirstOrDefault(n => n.Title == "Title4");

            var response = httpClient.DeleteAsync("api/news/" + newsToDelete.Id).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(this.repo.All().Count() == 3);
        }

        [TestMethod]
        public void Delete_News_With_Invalid_Data_Should_Return_400BadRequest()
        {
            var news = new List<News>()
            {
                new News() { Title = "Title1", Content = "Content1", PublishDate = DateTime.Now },
                new News() { Title = "Title2", Content = "Content2", PublishDate = DateTime.Now },
                new News() { Title = "Title3", Content = "Content3", PublishDate = DateTime.Now },
            };

            foreach (var newsItem in news)
            {
                this.repo.Add(newsItem);
            }

            this.repo.SaveChanges();

            var response = httpClient.DeleteAsync("api/news/" + -1).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(this.repo.All().Count() == 3);
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
