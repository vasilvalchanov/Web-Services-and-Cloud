using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Data.RepositoryModels
{
    using System.Data.Entity;

    using OnlineShop.Data.Interfaces;
    using OnlineShop.Models;

    public class OnlineShopData : IOnlineShopData
    {
        private DbContext contex;

        private IDictionary<Type, object> repositories;

        public OnlineShopData(DbContext context)
        {
            this.contex = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Ad> Ads
        {
            get
            {
                return this.GetRepository<Ad>();
            }
        }

        public IRepository<AdType> AdTypes
        {
            get
            {
                return this.GetRepository<AdType>();
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                return this.GetRepository<Category>();
            }
        }

        public IRepository<ApplicationUser> Users
        {
            get
            {
                return this.GetRepository<ApplicationUser>();
            }
        }

        public int SaveChanges()
        {
            return this.contex.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!this.repositories.ContainsKey(type))
            {
                var typeOfRepository = typeof(GenericRepository<T>);
                var repository = Activator.CreateInstance(typeOfRepository, this.contex);
                this.repositories.Add(type, repository);
            }
            return (IRepository<T>)this.repositories[type];
        }
    }
}
