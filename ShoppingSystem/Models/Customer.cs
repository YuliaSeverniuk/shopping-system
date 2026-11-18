using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Customer
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [Range(0, 100)]
        public int? Discount { get; set; }

        [Display(Name = "Customer")]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
