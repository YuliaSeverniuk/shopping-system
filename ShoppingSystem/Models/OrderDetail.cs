using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }

        [Display(Name = "Order")]
        public int? OrderId { get; set; }
        public Order Order { get; set; }

        [Display(Name = "Product")]
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        [Range(0.001, double.MaxValue)]
        public double? Quantity { get; set; }

        [Display(Name = "Cost")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Cost => (Product?.Price ?? 0) * (Quantity ?? 0);
    }
}
