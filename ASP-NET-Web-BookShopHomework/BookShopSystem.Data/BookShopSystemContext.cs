namespace BookShopSystem.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using BookShopSystem.Data.Migrations;
    using BookShopSystem.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class BookShopSystemContext : IdentityDbContext<ApplicationUser>
    {
       
        public BookShopSystemContext()
            : base("name=BookShopSystemContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BookShopSystemContext, Configuration>());
        }

        public virtual IDbSet<Category> Categories { get; set; }

        public virtual IDbSet<Book> Books { get; set; }

        public virtual IDbSet<Author> Authors { get; set; }

        public virtual IDbSet<Purchase> Purchases { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany(b => b.RelatedBooks)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("BookId");
                    m.MapRightKey("RelatedId");
                    m.ToTable("RelatedBooks");
                });

            base.OnModelCreating(modelBuilder);
        }

        public static BookShopSystemContext Create()
        {
            return new BookShopSystemContext();
        }

    }
}