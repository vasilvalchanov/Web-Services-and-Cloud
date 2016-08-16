using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services.Models.CustomAttributes
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class MinElementsInCollectionAttribute : ValidationAttribute
    {
        private readonly int minElements;

        public MinElementsInCollectionAttribute(int minElements)
        {
            this.minElements = minElements;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count >= this.minElements;
            }


            return false;
        }
    }
}