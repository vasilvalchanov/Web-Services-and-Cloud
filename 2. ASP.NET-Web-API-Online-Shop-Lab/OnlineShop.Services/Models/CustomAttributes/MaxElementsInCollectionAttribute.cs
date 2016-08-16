using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services.Models.CustomAttributes
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;

    public class MaxElementsInCollectionAttribute : ValidationAttribute
    {
        private readonly int maxElements;

        public MaxElementsInCollectionAttribute(int maxElements)
        {
            this.maxElements = maxElements;
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count <= this.maxElements;
            }


            return false;
        }
    }
}