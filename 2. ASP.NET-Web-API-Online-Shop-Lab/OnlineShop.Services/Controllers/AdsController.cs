using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services.Controllers
{
    using System.Runtime.InteropServices.ComTypes;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;

    using OnlineShop.Models;
    using OnlineShop.Services.Models.BindingModels;
    using OnlineShop.Services.Models.ViewModels;

    [Authorize]
    public class AdsController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetAds()
        {
            var ads =
                this.Data.Ads.Where(a => a.Status == AdStatus.Open)
                    .OrderByDescending(a => a.Type.Id)
                    .ThenBy(a => a.PostedOn)
                    .Select(
                        a =>
                        new
                            {
                                AdId = a.Id,
                                AdName = a.Name,
                                AdDescription = a.Description,
                                AdPrice = a.Price,
                                AdOwnerId = a.OwnerId,
                                AdOwnerUsername = a.Owner.UserName,
                                AdType = a.Type.Name,
                                AdDate = a.PostedOn,
                                AdCategories = a.Categories.Select(c => new { CatId = c.Id, CatName = c.Name })
                            });

            return this.Ok(ads);
        }

        [HttpPost]
        public IHttpActionResult CreateAd(CreateAdBindingModel model)
        {
            var userId = this.User.Identity.GetUserId();
            //if (userId == null)
            //{
            //    return this.Unauthorized();
            //}

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (!this.Data.AdTypes.Any(at => at.Id == model.TypeId))
            {
                return this.BadRequest("Invalid AdTypes");
            }

            var ad = new Ad()
                         {
                             Name = model.Name,
                             Description = model.Description,
                             OwnerId = userId,
                             Price = model.Price,
                             PostedOn = DateTime.Now,
                             Status = AdStatus.Open,
                             TypeId = model.TypeId
                         };

            foreach (var catId in model.Categories)
            {
                var category = this.Data.Categories.Find(catId);

                if (category == null)
                {
                    return this.BadRequest("There aren't such categories in the database");
                }

                ad.Categories.Add(category);
            }

            this.Data.Ads.Add(ad);
            this.Data.SaveChanges();

            var result = this.Data.Ads.Where(a => a.Id == ad.Id).Select(AdViewModel.Create).FirstOrDefault();

            return this.Ok(result);
        }

        [HttpPut]
        [Route("api/ads/{id}/close")]
        public IHttpActionResult CloseAd(int id)
        {
            var ad = this.Data.Ads.Find(id);

            if (ad == null)
            {
                return this.BadRequest("There isn't an ad with such id.");
            }

            var userId = this.User.Identity.GetUserId();

            if (ad.OwnerId != userId)
            {
                return this.BadRequest("You are not owner of this ad and cannot close it.");
            }

            ad.Status = AdStatus.Closed;
            ad.ClosedOn = DateTime.Now;
            this.Data.SaveChanges();

            return this.Ok();
        }
    }
}