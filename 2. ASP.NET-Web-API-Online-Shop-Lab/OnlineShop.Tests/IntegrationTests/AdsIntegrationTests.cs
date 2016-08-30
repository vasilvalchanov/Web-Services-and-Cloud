using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Tests.IntegrationTests
{
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Web.Http;

    using EntityFramework.Extensions;

    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OnlineShop.Data;
    using OnlineShop.Models;
    using OnlineShop.Services;
    using OnlineShop.Services.Models.BindingModels;

    using Owin;

    [TestClass]
    public class AdsIntegrationTests
    {
        private const string TestUserUsername = "username";
        private const string TestUserPassword = "paSs123#word";

        private static TestServer httpTestServer;

        private static HttpClient httpclient;

        private string accessToken;

        private string AccessToken
        {
            get
            {
                if (this.accessToken == null)
                {
                    var loginRespone = this.Login();
                    if (!loginRespone.IsSuccessStatusCode)
                    {
                        Assert.Fail("Unable to login: " + loginRespone.ReasonPhrase);
                    }

                    var loginData = loginRespone.Content.ReadAsAsync<LoginDTO>().Result;
                    this.accessToken = loginData.Access_Token;
                }

                return this.accessToken;
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            CleanDatabase();

            httpTestServer = TestServer.Create(
                appBuilder =>
                    {
                        var config = new HttpConfiguration();
                        WebApiConfig.Register(config);

                        var startUp = new Startup();

                        startUp.Configuration(appBuilder);
                        appBuilder.UseWebApi(config);
                    });

            httpclient = httpTestServer.HttpClient;

            SeedDatabase();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }

            CleanDatabase();
        }

        [TestMethod]
        public void Login_Should_Return_200OK_And_AccessToken()
        {
            var loginResponse = this.Login();
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var responseContent = loginResponse.Content.ReadAsAsync<LoginDTO>().Result;
            var accessToken = responseContent.Access_Token;

            Assert.IsNotNull(accessToken);
        }

        [TestMethod]
        public void Post_New_Ad_Should_Return_200OK()
        {
            var context = new OnlineShopContext();
            var category = context.Categories.FirstOrDefault(at => at.Name == "Cars").Id;
            var type = context.AdTypes.FirstOrDefault();



            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Trabant - sporten"), 
               new KeyValuePair<string, string>("description", "Lqta djanta i muzika :)"),
               new KeyValuePair<string, string>("Price", "1500"),
               new KeyValuePair<string, string>("categories[0]", category.ToString()),
               new KeyValuePair<string, string>("typeId", type.Id.ToString()),
            });

            var response = this.PostNewAd(content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void CreateAd_Invalid_Data_Should_Return_400BadRequest()
        {
            var context = new OnlineShopContext();
            var category = context.Categories.FirstOrDefault(at => at.Name == "Cars").Id;
            var type = context.AdTypes.FirstOrDefault();

            var content = new FormUrlEncodedContent(new[]
            {
               new KeyValuePair<string, string>("description", "Lqta djanta i muzika :)"),
               new KeyValuePair<string, string>("Price", "1500"),
               new KeyValuePair<string, string>("categories[0]", category.ToString()),
               new KeyValuePair<string, string>("typeId", type.Id.ToString()),
            });

            var response = this.PostNewAd(content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Post_New_Ad_Invalid_Type_Should_Return_400BadRequest()
        {
            var context = new OnlineShopContext();
            var category = context.Categories.FirstOrDefault(at => at.Name == "Cars").Id;
          

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Trabant - sporten"),
               new KeyValuePair<string, string>("description", "Lqta djanta i muzika :)"),
               new KeyValuePair<string, string>("Price", "1500"),
               new KeyValuePair<string, string>("categories[0]", category.ToString()),
               new KeyValuePair<string, string>("typeId", "-1"),
            });

            var response = this.PostNewAd(content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void Post_New_Ad_Invalid_Catergory_Should_Return_400BadRequest()
        {
            var context = new OnlineShopContext();
            var type = context.AdTypes.FirstOrDefault();



            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Trabant - sporten"),
               new KeyValuePair<string, string>("description", "Lqta djanta i muzika :)"),
               new KeyValuePair<string, string>("Price", "1500"),
               new KeyValuePair<string, string>("categories[0]", "Srabska muzika"),
               new KeyValuePair<string, string>("typeId", type.Id.ToString()),
            });

            var response = this.PostNewAd(content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private HttpResponseMessage PostNewAd(FormUrlEncodedContent content)
        {
            httpclient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AccessToken);

            var respose = httpclient.PostAsync("/api/ads", content).Result;

            return respose;
        }

        private static void SeedDatabase()
        {
            var context = new OnlineShopContext();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);

            var user = new ApplicationUser()
                           {
                               UserName = TestUserUsername,
                               Email = "testUser@email.com"
                           };

            var result = userManager.CreateAsync(user, TestUserPassword).Result;
            if (!result.Succeeded)
            {
                Assert.Fail(string.Join(Environment.NewLine, result.Errors));
            }

            SeedCategories(context);
            SeedAdTypes(context);
        }

        private static void CleanDatabase()
        {
            var context = new OnlineShopContext();

            context.Ads.Delete();
            context.AdTypes.Delete();
            context.Categories.Delete();
            context.Users.Delete();
        }

        private static void SeedCategories(OnlineShopContext context)
        {
            context.Categories.Add(new Category { Name = "Cars" });
            context.Categories.Add(new Category { Name = "Phones" });
            context.Categories.Add(new Category { Name = "Cameras" });
            context.SaveChanges();
        }

        private static void SeedAdTypes(OnlineShopContext context)
        {
            context.AdTypes.Add(new AdType { Name = "Normal", Index = 100 });
            context.AdTypes.Add(new AdType { Name = "Premium", Index = 200 });
            context.AdTypes.Add(new AdType { Name = "Gold", Index = 300 });
            context.SaveChanges();
        }

        private HttpResponseMessage Login()
        {
            var loginData = new FormUrlEncodedContent(new []
            {
                 new KeyValuePair<string, string>("username", TestUserUsername),
                 new KeyValuePair<string, string>("password", TestUserPassword),
                 new KeyValuePair<string, string>("grant_type", "password"),                                                
            });

            var response = httpclient.PostAsync("/token", loginData).Result;

            return response;
        }
    }
}
