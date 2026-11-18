using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShoppingSystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "OrderDate")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Display(Name = "SuperMarket")]
        public int SuperMarketId { get; set; }
        [Display(Name = "SuperMarket")]
        public Supermarket SuperMarket { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public double Total => OrderDetails.Sum(d => d.Cost);
    }
}
