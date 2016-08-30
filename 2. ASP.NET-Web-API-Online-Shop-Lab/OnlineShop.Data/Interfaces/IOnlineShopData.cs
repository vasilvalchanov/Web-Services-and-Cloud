﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Data.Interfaces
{
    using OnlineShop.Models;

    public interface IOnlineShopData
    {
        IRepository<Ad> Ads { get; }

        IRepository<AdType> AdTypes { get; }
        
        IRepository<Category> Categories { get; }

        IRepository<ApplicationUser> Users { get; } 

        int SaveChanges();
    }
}
