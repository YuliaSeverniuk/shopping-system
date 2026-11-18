using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Range(0, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Price { get; set; }

        [Display(Name = "Product")]
        public string NameWithPrice => $"{Name} ({Price:0.00})";

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
