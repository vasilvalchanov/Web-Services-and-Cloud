using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserPersonalMessageViewModel : AnonymousPersonalMessageViewModel
    {
        [Required]
        public string Sender { get; set; }
    }
}