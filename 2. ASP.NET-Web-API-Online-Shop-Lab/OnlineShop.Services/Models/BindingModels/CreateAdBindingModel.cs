using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    using OnlineShop.Services.Models.CustomAttributes;

    public class CreateAdBindingModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "The {0} must be at least {2} long")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int TypeId { get; set; }

        public decimal Price { get; set; }

        [MinElementsInCollection(1, ErrorMessage = "There must be at least 1 category")]
        [MaxElementsInCollection(3, ErrorMessage = "There must be maximum 3 categories")]
        public IEnumerable<int> Categories { get; set; }

    }
}