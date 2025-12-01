using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    class Quote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<QuoteItem> Items
        {
            get; set;
        }
    }
}
