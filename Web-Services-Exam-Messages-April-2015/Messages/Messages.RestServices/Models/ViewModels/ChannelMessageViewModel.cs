using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models.ViewModels
{
    using System.Linq.Expressions;

    using Messages.Data.Models;

    public class ChannelMessageViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string Sender { get; set; }

        public static Expression<Func<ChannelMessage, ChannelMessageViewModel>> Create
        {
            get
            {
                return cm => new ChannelMessageViewModel()
                {
                    Id = cm.Id,
                    Text = cm.Text,
                    DateSent = cm.DateSent,
                    Sender = cm.User == null ? null : cm.User.UserName
                };
            }
        }
    }
}