 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;

    using OnlineShop.Data;

    public class BaseApiController : ApiController
    {
        public BaseApiController(OnlineShopContext data)
        {
            this.Data = data;
        }

        public BaseApiController() : this(new OnlineShopContext())
        {          
        }

        protected OnlineShopContext Data { get; set; }
    }
}