using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class EditChannelBindingModel
    {
        [Required]
        public string Name { get; set; }
    }
}