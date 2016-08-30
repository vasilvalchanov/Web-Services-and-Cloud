 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;

    using OnlineShop.Data;
    using OnlineShop.Data.Interfaces;
    using OnlineShop.Data.RepositoryModels;
    using OnlineShop.Tests;

    public class BaseApiController : ApiController
    {
        public BaseApiController(IOnlineShopData data, IUserIdProvider userIdProvider)
        {
            this.Data = data;
            this.UserIdProvider = userIdProvider;
        }

        public BaseApiController() : this(new OnlineShopData(new OnlineShopContext()), new AspNetUserIdProvider())
        {          
        }

        protected IOnlineShopData Data { get; set; }

        protected IUserIdProvider UserIdProvider { get; set; }
    }
}