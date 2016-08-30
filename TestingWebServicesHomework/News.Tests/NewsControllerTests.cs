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
    using System.Web.Http.Results;
    using System.Web.Http.Routing;

    using News.Models;
    using News.Services.Controllers;
    using News.Services.Models;

    [TestClass]
    public class NewsControllerTests
    {
        private NewsRepositoryMock repo;

        [TestInitialize]
        public void TestInitialize()
        {
            this.repo = new NewsRepositoryMock(); // create fake repo

            var listOfNews = new List<News>()  // create some news to test with them
            {
                new News()
                    {
                        Id = 1,
                        Title = "Title1",
                        Content = "Content1",
                        PublishDate = DateTime.Now
                    },
                new News()
                    {
                            Id = 2,
                        Title = "Title2",
                        Content = "Content2",
                        PublishDate = DateTime.Now
                    },
                new News()
                    {
                            Id = 3,
                        Title = "Title3",
                        Content = "Content3",
                        PublishDate = DateTime.Now
                    }
              };

            this.repo.News = listOfNews; // assign this collection of news to repo News(List<News>)

        }

        [TestMethod]
        public void Get_All_News_Should_Return_Them_Correctly()
        {
 
            var controller = new NewsController(this.repo);  // create new NewsController and inject our repo
            var result = controller.GetNews();  // call the method that we test (in this case Getnews())

            var fakeNews = this.repo.News.OrderByDescending(n => n.PublishDate).ToList();
        
            CollectionAssert.AreEquivalent(fakeNews, result.ToList());

        }

        [TestMethod]
        public void Create_News_With_Valid_Data_Should_Return_It_Correctly()
        {
            var newsToAdd = new NewsBindingModel()  // create news to Add
                                {
                                    Title = "Baba Gicka President",
                                    Content = "Baba Gicka.....",
                                    
                                };


            var controller = new NewsController(this.repo);  // create new NewsController and inject our repo
            this.SetupController(controller, "news"); // this method is necessary when we work
                                                      // with HttpActionResult

            // this give us a response from our request
            var response = controller.CreateNews(newsToAdd).ExecuteAsync(new CancellationToken()).Result;
            
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode); // we check the status code
            Assert.IsNotNull(response.Headers.Location); // check if the location is not null

            //take the response content as News object
            var newsFromResponse = response.Content.ReadAsAsync<News>().Result;
            var newsFromDb = this.repo.All().FirstOrDefault(n => n.Title == newsToAdd.Title);

            // Assert the service response values are correct  as check our News props
            Assert.AreEqual(newsFromDb.Title, newsFromResponse.Title);
            Assert.AreEqual(newsFromDb.Content, newsFromResponse.Content);
            Assert.AreEqual(newsFromDb.PublishDate, newsFromResponse.PublishDate);

            // Assert the repository values are correct
            Assert.IsTrue(this.repo.News.Count == 4);

        }

        [TestMethod]
        public void Create_News_With_Invalid_Data_Should_Return_BadRequest()
        {
            var newsToAdd = new NewsBindingModel()
            {
                Title = "Baba Gicka President",
                Content = null
            };


            var controller = new NewsController(this.repo);
            this.SetupController(controller, "news");

            // to receive Bad Request for my invalid data in binding model should do this:
            controller.ModelState.AddModelError("Content", "A value is required.");
            var response = controller.CreateNews(newsToAdd).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(this.repo.News.Count == 3);
        }

        [TestMethod]
        public void Edit_Existing_News_Invalid_Data_Should_Return_400BadRequest()
        {
            var newsToEdit = this.repo.News.FirstOrDefault();
            var newsToEditId = newsToEdit.Id;
            var editedNews = new NewsBindingModel()
                                 {
                                     Title = null,
                                     Content = "Content1"
                                 };

            var controller = new NewsController(this.repo);
            this.SetupController(controller, "news");
            controller.ModelState.AddModelError("Title", "Value cannot be null");

            var response =
                controller.UpdateNewsById(newsToEditId, editedNews).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);


        }

        [TestMethod]
        public void Edit_Existing_News_ValidData_Should_Return_200OK_And_Edit_It()
        {
            var newsToEdit = this.repo.News.FirstOrDefault();
            var newsToEditId = newsToEdit.Id;
            var editedNews = new NewsBindingModel()
            {
                Title = "Smotana rabota",
                Content = "Content1"
            };

            var controller = new NewsController(this.repo);
            this.SetupController(controller, "news");


            var response =
                controller.UpdateNewsById(newsToEditId, editedNews).ExecuteAsync(new CancellationToken()).Result;

            var editedNewsFromDb = this.repo.News.FirstOrDefault(n => n.Id == newsToEditId);
            var editedNewsFromResponse = response.Content.ReadAsAsync<News>().Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("Smotana rabota", editedNewsFromResponse.Title);
            Assert.AreEqual("Content1", editedNewsFromResponse.Content);
            Assert.AreEqual(newsToEdit.PublishDate, editedNewsFromResponse.PublishDate);

            Assert.AreEqual(editedNewsFromDb.Title, editedNewsFromResponse.Title);
            Assert.AreEqual(editedNewsFromDb.Content, editedNewsFromResponse.Content);
            Assert.AreEqual(newsToEdit.PublishDate, editedNewsFromResponse.PublishDate);


        }

        [TestMethod]
        public void Edit_NonExisting_News_Should_Return_400BadRequest()
        {
            var newsToEditId = this.repo.News.Where(n => n.Content == "nqma takava").Select(n => n.Id).FirstOrDefault();

            var controller = new NewsController(this.repo);
            this.SetupController(controller, "news");

            var editedNewsModel = new NewsBindingModel()
            {
                Title = "Smotana rabota",
                Content = "Content1"
            };

            var response = controller.UpdateNewsById(newsToEditId, editedNewsModel).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Delete_News_Return_200OK_And_Delete_It_Successfully()
        {
            var newsToDelete = this.repo.News.FirstOrDefault();

            var controller = new NewsController(this.repo);
            this.SetupController(controller, "news");

            var response = controller.DeleteNews(newsToDelete.Id).ExecuteAsync(new CancellationToken()).Result;
            var deletedNews = this.repo.News.FirstOrDefault(n => n.Title == "Title1");


            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.IsTrue(this.repo.News.Count == 2);
            Assert.IsNull(deletedNews);
        }

        [TestMethod]
        public void Delete_NonExisting_News_Should_Return_400BadRequest()
        {
            var newsToDeleteId = this.repo.News.Where(n => n.Content == "nqma takava").Select(n => n.Id).FirstOrDefault();

            var controller = new NewsController(this.repo);
            this.SetupController(controller, "news");

            var response = controller.DeleteNews(newsToDeleteId).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(this.repo.News.Count == 3);
        }



        private void SetupController(ApiController controller, string controllerName)
        {
            // Setup the Request object of the controller
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://sample-url.com")
            };
            controller.Request = request;

            // Setup the configuration of the controller
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi", routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            // Apply the routes to the controller
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", controllerName } });
        }
    }
}
