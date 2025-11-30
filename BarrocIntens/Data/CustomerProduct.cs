using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    class CustomerProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public DateTime StartLeaseDate { get; set; }
        public DateTime EndLeaseDate { get; set; }

        // Source ChatGPT
        public CustomerProduct()
        {
            StartLeaseDate = DateTime.Now;
            EndLeaseDate = StartLeaseDate.AddYears(1);
        }
    }
}
