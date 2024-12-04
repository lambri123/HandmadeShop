using HandmadeShop.Models;

namespace HandmadeShop.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            
            context.Database.EnsureCreated();

           
            if (context.Products.Any())
            {
                return; 
            }

           
            var products = new Product[]
            {
                new Product { Name = "Handmade Necklace", Description = "Beautiful handmade necklace.", Price = 29.99m, Stock = 10, Category = "Jewelry" },
                new Product { Name = "Wooden Toy", Description = "Eco-friendly wooden toy.", Price = 19.99m, Stock = 20, Category = "Toys" },
                new Product { Name = "Knitted Scarf", Description = "Warm and cozy scarf.", Price = 24.99m, Stock = 15, Category = "Accessories" }
            };

            foreach (var product in products)
            {
                context.Products.Add(product);
            }
            context.SaveChanges();
        }
    }
}
