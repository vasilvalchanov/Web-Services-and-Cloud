namespace OnlineShop.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;

    using Microsoft.AspNet.Identity.EntityFramework;

    using OnlineShop.Data.Migrations;
    using OnlineShop.Models;

    public class OnlineShopContext : IdentityDbContext<ApplicationUser>
    {

        public OnlineShopContext()
            : base("name=OnlineShopContext")
        {
            
        }

        public virtual IDbSet<Category> Categories { get; set; }

        public virtual IDbSet<Ad> Ads { get; set; }

        public virtual IDbSet<AdType> AdTypes { get; set; }

        public static OnlineShopContext Create()
        {
            return new OnlineShopContext();
        }
    }
}