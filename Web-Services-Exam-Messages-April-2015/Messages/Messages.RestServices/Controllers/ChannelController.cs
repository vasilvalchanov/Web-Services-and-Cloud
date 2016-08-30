using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Messages.RestServices.Controllers
{
    using System.Data.Entity;

    using Messages.Data;
    using Messages.Data.Models;
    using Messages.RestServices.Models.BindingModels;
    using Messages.RestServices.Models.ViewModels;

    [RoutePrefix("api/channels")]
    public class ChannelsController : ApiController
    {
        private MessagesDbContext context = new MessagesDbContext();

        [HttpGet]
        public IHttpActionResult GetAllChannels()
        {
            var channels = this.context.Channels.OrderBy(c => c.Name);

            var result = channels.Select(ChannelViewModel.Create);

            return this.Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetChannelById(int id)
        {
            var channel = this.context.Channels.Find(id);

            if (channel == null)
            {
                return this.NotFound();
            }

            var result = new ChannelViewModel()
                             {
                                 Id = channel.Id,
                                 Name = channel.Name
                             };

            return this.Ok(result);
        }

        [HttpPost]
        public IHttpActionResult CreateChannel([FromBody]CreateChannelBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.context.Channels.Any(c => c.Name == model.Name))
            {
                return this.Conflict();
            }

            var channel = new Channel()
                              {
                                  Name = model.Name
                              };

            this.context.Channels.Add(channel);
            this.context.SaveChanges();

            var createdChannelId = this.context.Channels.Find(channel.Id).Id;

            var location = "http://localhost:7777/api/channels/" + createdChannelId;
            var result = new ChannelViewModel()
                             {
                                 Id = channel.Id,
                                 Name = channel.Name
                             };

            return this.Created(location, result);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditExistingChannelById([FromUri]int id, [FromBody]EditChannelBindingModel model)
        {
            var channelToEdit = this.context.Channels.Find(id);

            if (channelToEdit == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest();
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (this.context.Channels.Any(c => c.Name == model.Name && c.Id != id))
            {
                return this.Conflict();
            }

            channelToEdit.Name = model.Name;
            this.context.SaveChanges();

            var result = new MessageViewModel()
                             {
                                 Message = "Channel #" + id + " edited successfully."
                             };

            return this.Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteChannelById(int id)
        {
            var channelToDelete = this.context.Channels.Find(id);

            if (channelToDelete == null)
            {
                return this.NotFound();
            }

            if (channelToDelete.ChannelMessages.Any())
            {
                return this.Conflict();
            }

            this.context.Channels.Remove(channelToDelete);
            this.context.SaveChanges();

            var result = new MessageViewModel()
                             {
                                 Message = "Channel #" + id + " deleted."
            };

            return this.Ok(result);
        }
    }
}
