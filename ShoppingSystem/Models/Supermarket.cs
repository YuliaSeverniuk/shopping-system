using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Supermarket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
