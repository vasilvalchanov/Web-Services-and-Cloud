using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Messages.RestServices.Controllers
{
    using Messages.Data;
    using Messages.Data.Models;
    using Messages.RestServices.Models.BindingModels;
    using Messages.RestServices.Models.ViewModels;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/channel-messages")]
    public class ChannelMessagesController : ApiController
    {
        private MessagesDbContext context = new MessagesDbContext();

        [HttpGet]
        [Route("{channelName}")]
        public IHttpActionResult GetMessagesByChannelId(string channelName)
        {
            var channel = this.context.Channels.FirstOrDefault(c => c.Name == channelName);

            if (channel == null)
            {
                return this.NotFound();
            }

            var channelMessages = channel.ChannelMessages.OrderByDescending(cm => cm.DateSent)
                .AsQueryable()
                .Select(ChannelMessageViewModel.Create);

            return this.Ok(channelMessages);

        }

        [HttpGet]
        [Route("{channelName}")]
        public IHttpActionResult GetMessagesByChannelIdLimit(string channelName, [FromUri]int limit)
        {
            var channel = this.context.Channels.FirstOrDefault(c => c.Name == channelName);

            if (channel == null)
            {
                return this.NotFound();
            }

            if (limit < 1 || limit > 1000)
            {
                return this.BadRequest();
            }

            var channelMessages = channel.ChannelMessages.OrderByDescending(cm => cm.DateSent).Take(limit)
                .AsQueryable()
                .Select(ChannelMessageViewModel.Create);

            return this.Ok(channelMessages);

        }

        [HttpPost]
        [Route("{channelName}")]
        public IHttpActionResult CreateAnonymousMsgToExistingChannel([FromUri]string channelName, [FromBody]CreateMessageBindingModel model)
        {
            var channel = this.context.Channels.FirstOrDefault(c => c.Name == channelName);

            if (channel == null)
            {
                return this.NotFound();
            }

            var message = new ChannelMessage()
                              {
                                  Text = model.Text,
                                  DateSent = DateTime.Now,
                                  Channel = channel,
                                  UserId = null
                              };

            var user = this.User.Identity.GetUserId();

            if (user != null)
            {
                message.UserId = user;
            }
           
            channel.ChannelMessages.Add(message);
            this.context.SaveChanges();


            if (user == null)
            {
                var result = new MessageChannelWithoutUserViewModel()
                             {
                                 Id = message.Id,
                                 Message = "Anonymous message sent to channel " + channelName
                             };

                return this.Ok(result);
            }
            else
            {
                var loggedUser = this.context.Users.First(u => u.Id == user);

                var result = new MessageChannelWithUserViewModel()
                                 {
                                     Id = message.Id,
                                     Sender = loggedUser.UserName,
                                     Message = "Message sent to channel " + channelName + "."
                };

                return this.Ok(result);
            }
        }
    }
}
