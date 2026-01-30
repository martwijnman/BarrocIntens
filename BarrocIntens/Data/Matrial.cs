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
    public class Matrial
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public string ImagePath => $"ms-appx:///Assets/{Image}";
        public int Stock { get; set; }
        public int MinimumStock { get; set; }
        public int Sold { get; set; }
        public int Quantity { get; set; }
        public int OrderAmount { get; set; } = 0;
        [Required]
        public bool NotificationOutOfStock { get; set; }
        public string NotificationSymbol
        {
            get { return NotificationOutOfStock ? "○" : "●"; }
        }
        public Brush NotificationColor =>
            NotificationOutOfStock ? new SolidColorBrush(Colors.LimeGreen)
                   : new SolidColorBrush(Colors.Red);
    }
}
