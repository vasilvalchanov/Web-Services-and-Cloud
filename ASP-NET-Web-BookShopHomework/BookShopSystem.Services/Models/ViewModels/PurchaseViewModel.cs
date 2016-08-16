using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Models.ViewModels
{
    using BookShopSystem.Models;
    using BookShopSystem.Services.Models.BindingModels;

    public class PurchaseViewModel
    {
        public string BookTitle { get; set; }

        public decimal PurchasePrice { get; set; }

        public DateTime DateOfPurchase { get; set; }

        public bool IsRecalled { get; set; }

        public static PurchaseViewModel Create(Purchase purchase)
        {
            var purchaseView = new PurchaseViewModel()
                                   {
                                       BookTitle = purchase.Book.Title,
                                       PurchasePrice = purchase.Price,
                                       DateOfPurchase = purchase.DateOfPurchase,
                                       IsRecalled = purchase.IsRecalled
                                   };

            return purchaseView;
        }
    }
}