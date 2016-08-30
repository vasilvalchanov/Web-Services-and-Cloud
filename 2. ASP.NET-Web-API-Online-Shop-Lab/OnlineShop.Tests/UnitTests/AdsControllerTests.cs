using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OnlineShop.Tests.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Routing;

    using Moq;

    using OnlineShop.Data.Interfaces;
    using OnlineShop.Data.RepositoryModels;
    using OnlineShop.Models;
    using OnlineShop.Services.Controllers;
    using OnlineShop.Services.Models.BindingModels;
    using OnlineShop.Services.Models.ViewModels;

    [TestClass]
    public class AdsControllerTests
    {
        private MockContainer mocks;

        [TestInitialize]
        public void TestInit()
        {
            this.mocks = new MockContainer();
            this.mocks.PrepareMocks();
        }

        [TestMethod]
        public void GetAllAds_Should_Return_Total_Ads_SortedBy_Type_Index()
        {
            var fakeAds = this.mocks.AdRepositoryMock.Object.All();

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads.All()).Returns(fakeAds);

            var mockIdUserProvider = new Mock<IUserIdProvider>();

            var adsController = new AdsController(mockContext.Object, mockIdUserProvider.Object);

            this.SetupController(adsController, "ads");

            var response = adsController.GetAds().ExecuteAsync(CancellationToken.None).Result;

            var adsResponse = response.Content.ReadAsAsync<IEnumerable<AdViewModel>>().Result.Select(a => a.Id).ToList();

            var orderedFakeAds = fakeAds.OrderBy(a => a.Type.Index).ThenBy(a => a.PostedOn).Select(a => a.Id).ToList();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            CollectionAssert.AreEqual(orderedFakeAds, adsResponse);
        }

        [TestMethod]
        public void Create_Ad_Should_Successfully_Add_To_The_Repository()
        {
            var ads = new List<Ad>();

            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();

            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available");
            }

            this.mocks.AdRepositoryMock.Setup(r => r.Add(It.IsAny<Ad>())).Callback(
                (Ad ad) =>
                    {
                        ad.Owner = fakeUser;
                        ads.Add(ad);
                    });

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads).Returns(this.mocks.AdRepositoryMock.Object);
            mockContext.Setup(c => c.AdTypes).Returns(this.mocks.AdTypeRepositoryMock.Object);
            mockContext.Setup(c => c.Categories).Returns(this.mocks.CategoryRepositoryMock.Object);
            mockContext.Setup(c => c.Users).Returns(this.mocks.UserRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(c => c.GetUserId()).Returns(fakeUser.Id);

            var adsController = new AdsController(mockContext.Object, mockIdProvider.Object);

            this.SetupController(adsController, "ads");

            var randomName = Guid.NewGuid().ToString();

            var newAd = new CreateAdBindingModel()
            {
                Name = randomName,
                Price = 555,
                TypeId = 1,
                Description = "drun drun",
                Categories = new[] { 2, 3 }
            };

            var response = adsController.CreateAd(newAd).ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mockContext.Verify(c => c.SaveChanges(), Times.Once);

            Assert.IsTrue(ads.Count == 1);
            Assert.AreEqual(ads[0].Name, newAd.Name);
        }

        [TestMethod]
        public void Closing_Ad_As_Owner_Should_Return_200OK()
        {
            var fakeAds = this.mocks.AdRepositoryMock.Object.All();
            var openAd = fakeAds.FirstOrDefault(a => a.Status == AdStatus.Open);

            if (openAd == null)
            {
                Assert.Fail("Cannot perform test - no open ads available");
            }

            var adId = openAd.Id;

            var mockContex = new Mock<IOnlineShopData>();

            mockContex.Setup(c => c.Ads).Returns(this.mocks.AdRepositoryMock.Object);

            var mockUserIdProvider = new Mock<IUserIdProvider>();
            mockUserIdProvider.Setup(c => c.GetUserId()).Returns(openAd.OwnerId);

            var adsController = new AdsController(mockContex.Object, mockUserIdProvider.Object);
            this.SetupController(adsController, "ads");

            var response = adsController.CloseAd(adId).ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mockContex.Verify(c => c.SaveChanges(), Times.Once);

            Assert.IsNotNull(openAd.ClosedOn);
            Assert.IsTrue(openAd.Status == AdStatus.Closed);
        }

        [TestMethod]
        public void Closing_Ad_AsNonOwner_Should_Return_400BadRequest()
        {
            var openAd = this.mocks.AdRepositoryMock.Object.All().FirstOrDefault(a => a.Status == AdStatus.Open);

            if (openAd == null)
            {
                Assert.Fail("Cannot perform test - no open ads available");
            }

            var fakeUserProvider = new Mock<IUserIdProvider>();
            fakeUserProvider.Setup(m => m.GetUserId()).Returns(openAd.OwnerId + 1);

            var mockContex = new Mock<IOnlineShopData>();
            mockContex.Setup(c => c.Ads).Returns(this.mocks.AdRepositoryMock.Object);

            var adsController = new AdsController(mockContex.Object, fakeUserProvider.Object);
            this.SetupController(adsController, "ads");

            var response = adsController.CloseAd(openAd.Id).ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            mockContex.Verify(c => c.SaveChanges(), Times.Never);

            Assert.IsTrue(openAd.Status == AdStatus.Open);
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
