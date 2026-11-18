using System.Collections.Generic;

namespace ShoppingSystem.Models.ViewModels
{
    public class OrderIndexData
    {
        public IEnumerable<Order> Orders { get; set; }
        public Order SelectedOrder { get; set; }
    }
}
