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
    }
}
