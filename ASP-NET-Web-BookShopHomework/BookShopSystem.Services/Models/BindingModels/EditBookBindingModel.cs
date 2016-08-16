using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookShopSystem.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    using BookShopSystem.Models;

    public class EditBookBindingModel
    {
       
        [MinLength(1)]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal? Price { get; set; }

        public int? Copies { get; set; }

        public EditionType? EditionType { get; set; }

        public AgeRestriction? AgeRestriction { get; set; }

        public DateTime? ReliesDate { get; set; }

        public int? AuthorId { get; set; }

    }
}