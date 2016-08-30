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

    [RoutePrefix("api/user/personal-messages")]
    public class UserController : ApiController
    {
        private MessagesDbContext context = new MessagesDbContext();

        [HttpGet]
        [Authorize]
        public IHttpActionResult GetUserPersonalMessages()
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.context.Users.Find(userId);

            var messages = user.UserMessages.OrderByDescending(m => m.DateSent)
                .Select(m => new
                                 {
                                     Id = m.Id,
                                     Text = m.Text,
                                     DateSent = m.DateSent,
                                     Sender = m.OwnerUser == null ? null : m.OwnerUser.UserName
                                 });

            return this.Ok(messages);
        }

        [HttpPost]
        public IHttpActionResult PostAnonymousPersonalMessage([FromBody]CreatePersonalMessageBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var recipient = this.context.Users.FirstOrDefault(u => u.UserName == model.Recipient);

            if (recipient == null)
            {
                return this.BadRequest();
            }

            var message = new UserMessage()
                              {
                                  Text = model.Text,
                                  DateSent = DateTime.Now,
                                  RecipientUser = recipient,
                                  OwnerUser = null
                              };

            var senderId = this.User.Identity.GetUserId();

            if (senderId != null)
            {
                var sender = this.context.Users.Find(senderId);
                message.OwnerUser = sender;

                recipient.UserMessages.Add(message);
                this.context.SaveChanges();

                var result = new UserPersonalMessageViewModel()
                {
                    Id = message.Id,
                    Sender = sender.UserName,
                    Message = "Message sent successfully to user " + model.Recipient + "."
                };

                return this.Ok(result);
            }
            else
            {
                recipient.UserMessages.Add(message);
                this.context.SaveChanges();

                var result = new AnonymousPersonalMessageViewModel()
                {
                    Id = message.Id,
                    Message = "Anonymous message sent successfully to user " + model.Recipient + "."
                };

                return this.Ok(result);
            }

        }
    }
}
