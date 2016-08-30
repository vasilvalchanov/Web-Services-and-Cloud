namespace News.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Microsoft.AspNet.Identity.EntityFramework;

    using News.Data.Migrations;
    using News.Models;

    public class NewsContext : IdentityDbContext<ApplicationUser>
    {
       
        public NewsContext()
            : base("name=NewsContext")
        {
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<NewsContext, Configuration>());
        }

        public virtual IDbSet<News> News { get; set; }

        public static NewsContext Create()
        {
            return new NewsContext();
        }
    }
}