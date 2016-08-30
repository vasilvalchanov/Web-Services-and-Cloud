using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace News.Services.Models
{
    using System.ComponentModel.DataAnnotations;

    public class NewsBindingModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PublishedDate { get; set; }

    }
}