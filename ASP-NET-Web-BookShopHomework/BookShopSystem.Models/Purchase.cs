using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopSystem.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        public int BookId { get; set; }

        [Required]
        public virtual Book Book { get; set; }

        public virtual ApplicationUser User { get; set; }


        public string UserId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime DateOfPurchase { get; set; }

        [Required]
        public bool IsRecalled { get; set; }

    }
}
