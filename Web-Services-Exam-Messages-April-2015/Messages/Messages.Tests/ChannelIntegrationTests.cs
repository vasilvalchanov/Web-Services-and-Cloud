using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;

    using Messages.Data;
    using Messages.Data.Models;
    using Messages.Tests.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChannelIntegrationTests
    {
        private MessagesDbContext context;

        private HttpClient client;

        [TestInitialize]
        public void TestInit()
        {
            TestingEngine.CleanDatabase();
        }

        [TestMethod]
        public void Delete_Existing_EptyChannel_Should_Return_200OK()
        {
            var channel = new Channel()
                              {
                                  Name = "Haidede"
            };

            var response = TestingEngine.CreateChannelHttpPost(channel.Name);
            var createdChannel = response.Content.ReadAsAsync<ChannelModel>().Result;
            var deleteResponse = TestingEngine.HttpClient.DeleteAsync("api/channels/" + createdChannel.Id).Result;

            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.AreEqual(0, TestingEngine.GetChannelsCountFromDb());

        }

        [TestMethod]
        public void Delete_Existing_Channel_NonEmpty_Should_Return_Conflict()
        {
            var channel = new Channel()
            {
                Name = "Haidede"
            };

            var response = TestingEngine.CreateChannelHttpPost(channel.Name);
            
            Assert.AreEqual(1, TestingEngine.GetChannelsCountFromDb());

            var channelFromDb = response.Content.ReadAsAsync<ChannelModel>().Result;
            var sendingMessageResponse = TestingEngine.SendChannelMessageHttpPost(channel.Name, "baba Gicka");

            Assert.AreEqual(HttpStatusCode.OK, sendingMessageResponse.StatusCode);

            var deleteResponse = TestingEngine.HttpClient.DeleteAsync("api/channels/" + channelFromDb.Id).Result;

            Assert.AreEqual(HttpStatusCode.Conflict, deleteResponse.StatusCode);
            Assert.AreEqual(1, TestingEngine.GetChannelsCountFromDb());


        }

        [TestMethod]
        public void Delete_NonExisting_Channel_Should_Return_404NotFound()
        {
            var channel = new Channel()
            {
                Name = "Haidede"
            };

            var response = TestingEngine.CreateChannelHttpPost(channel.Name);

            Assert.AreEqual(1, TestingEngine.GetChannelsCountFromDb());

            var channelFromDb = response.Content.ReadAsAsync<ChannelModel>().Result;
            var sendingMessageResponse = TestingEngine.SendChannelMessageHttpPost(channel.Name, "baba Gicka");

            Assert.AreEqual(HttpStatusCode.OK, sendingMessageResponse.StatusCode);

            var deleteResponse = TestingEngine.HttpClient.DeleteAsync("api/channels/" + channelFromDb.Id + 1).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            Assert.AreEqual(1, TestingEngine.GetChannelsCountFromDb());


        }
    }
}
