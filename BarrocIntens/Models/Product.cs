using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="vul naam in")]
        [MaxLength(180)]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int MinimumStock { get; set; }

        [Required]
        public string Deliverer { get; set; }

        [Required]
        public bool NotificationOutOfStock { get; set; }

        [Required]
        public string Image { get; set; }
        // CRUD operations for Planning

        // Update
        public void Update(int productId, string name, string category, double price, int stock, int minimumStock, string deliverer, bool notificationOutOfStock, string image)
        {
            using (var db = new AppDbContext())
            {
                var planning = db.Products.FirstOrDefault(p => p.Id == productId);
                if (planning != null)
                {
                    planning.Name = name;
                    planning.Category = category;
                    planning.Price = price;
                    planning.Stock = stock;
                    planning.MinimumStock = minimumStock;
                    planning.Deliverer = deliverer;
                    planning.NotificationOutOfStock = notificationOutOfStock;
                    planning.Image = image;
                    db.SaveChanges();
                }
            }
        }

        // Delete
        public void Delete(int productId)
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
            }
        }
    }
}
