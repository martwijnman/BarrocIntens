using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
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
        [Range(0, 9999, ErrorMessage = "Stock moet tussen 1 en 9999 liggen")]
        public int Stock { get; set; }

        [Required]
        public int MinimumStock { get; set; }

        public int StockShortage => MinimumStock - Stock;

        [Required]
        public bool IsMachine { get; set; }

        [Required]
        public int DelivererId { get; set; }
        public Deliverer Deliverer { get; set; }

        [Required]
        public bool NotificationOutOfStock { get; set; }
        public string NotificationSymbol
        {
            get { return NotificationOutOfStock ? "○" : "●"; }
        }
        public Brush NotificationColor =>
            NotificationOutOfStock ? new SolidColorBrush(Colors.LimeGreen)
                   : new SolidColorBrush(Colors.Red);

        [Required]
        public string Image { get; set; }
        public string ImagePath => $"ms-appx:///Assets/{Image}";
        // CRUD operations for Planning

        // Update
        public void Update(int productId, string name, string category, double price, int stock, int minimumStock, int delivererId, bool notificationOutOfStock, string image)
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
                    planning.DelivererId = delivererId;
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
