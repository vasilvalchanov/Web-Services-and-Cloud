using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Controllers
{
    using System.Web.Http;

    using BookShopSystem.Data;

    public class BaseController : ApiController
    {
        public BaseController(BookShopSystemContext context)
        {
            this.Context = context;
        }

        public BaseController() : this(new BookShopSystemContext())
        {           
        }

        protected BookShopSystemContext Context { get; set; }
    }
}