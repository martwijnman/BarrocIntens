using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
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

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<QuoteItem> Items
        {
            get; set;
        }
        [AllowNull]
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }

        //public Facture Facture { get; set;  }
    }
}
