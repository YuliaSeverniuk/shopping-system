using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Models;

namespace ShoppingSystem.Data
{
    public static class SampleData
    {
        public static void Initialize(ShoppingContext context)
        {
            context.Database.Migrate();

            if (context.Customers.Any())
            {
                return;
            }

            var customers = new[]
            {
                new Customer { FirstName = "Shura",     LastName = "Balaganov",     Address = "Odessa",            Discount = 5 },
                new Customer { FirstName = "Ostap",     LastName = "Bender",        Address = "Rio de Zhmerinka",  Discount = 10 },
                new Customer { FirstName = "Madam",     LastName = "Gritsatsuieva", Address = "Ensk",              Discount = 0 },
                new Customer { FirstName = "Adam",      LastName = "Kozlevych",     Address = "Chernomorsk",       Discount = 3 },
                new Customer { FirstName = "Semiuel M.",LastName = "Panikovskyi",   Address = "Chernomorsk",       Discount = 0 },
                new Customer { FirstName = "Zosya",     LastName = "Sinitsina",     Address = "Chernomorsk",       Discount = 7 },
            };
            context.Customers.AddRange(customers);

            var supermarkets = new[]
            {
                new Supermarket { Name = "Billa",      Address = "Odessa" },
                new Supermarket { Name = "Big Pocket", Address = "Lviv" },
                new Supermarket { Name = "Wellmart",   Address = "Chernivtsi, Rus'ka" },
                new Supermarket { Name = "Silpo",      Address = "Kyiv, Khreshchatyk" },
                new Supermarket { Name = "ATB",        Address = "Dnipro" },
                new Supermarket { Name = "Novus",      Address = "Kharkiv" },
                new Supermarket { Name = "Fora",       Address = "Poltava" },
            };
            context.Supermarkets.AddRange(supermarkets);

            var products = new[]
            {
                new Product { Name = "Banana", Price = 45.50 },
                new Product { Name = "Butter", Price = 89.90 },
                new Product { Name = "Cola",   Price = 32.00 },
                new Product { Name = "Bread",  Price = 24.75 },
                new Product { Name = "Milk",   Price = 38.20 },
                new Product { Name = "Cheese", Price = 145.00 },
            };
            context.Products.AddRange(products);

            var order1 = new Order { OrderDate = new DateTime(2020, 8, 23), Customer = customers[0], SuperMarket = supermarkets[0] };
            var order2 = new Order { OrderDate = new DateTime(2020, 8, 25), Customer = customers[1], SuperMarket = supermarkets[2] };
            var order3 = new Order { OrderDate = new DateTime(2020, 9, 1),  Customer = customers[5], SuperMarket = supermarkets[1] };
            context.Orders.AddRange(order1, order2, order3);

            context.OrderDetails.AddRange(
                new OrderDetail { Order = order1, Product = products[0], Quantity = 1 },
                new OrderDetail { Order = order1, Product = products[1], Quantity = 2 },
                new OrderDetail { Order = order1, Product = products[2], Quantity = 1 },
                new OrderDetail { Order = order2, Product = products[3], Quantity = 2 },
                new OrderDetail { Order = order2, Product = products[5], Quantity = 0.5 },
                new OrderDetail { Order = order3, Product = products[4], Quantity = 3 });

            context.SaveChanges();
        }
    }
}
