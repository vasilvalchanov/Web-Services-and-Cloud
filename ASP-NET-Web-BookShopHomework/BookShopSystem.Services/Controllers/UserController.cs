using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Controllers
{
    using System.Web.Http;

    using BookShopSystem.Services.Models.ViewModels;

    [RoutePrefix("api/user")]
    public class UserController : BaseController
    {
        private ApplicationUserManager userManager;

        [HttpGet]
        [Route("{username}/purchases")]
        public IHttpActionResult GetAllPurchasesByUsername(string username)
        {
            var user = this.Context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return this.BadRequest("The user with such username does not exits.");
            }

            var result = user.Purchases.OrderBy(p => p.DateOfPurchase).Select(PurchaseViewModel.Create);

            return this.Ok(new
                               {
                                   Username = user.UserName,
                                   Purchases = result
                               });
        }
    }
}