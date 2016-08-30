using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace News.Services.Models
{
    public class LoginBindingModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string GrantType { get; set; }

    }
}