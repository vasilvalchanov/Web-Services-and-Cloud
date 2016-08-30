using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models.ViewModels
{
    public class MessageChannelWithUserViewModel : MessageChannelWithoutUserViewModel
    {
        public string Sender { get; set; }
    }
}