using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    class QuoteItem
    {
        [Key]
        public int Id { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }


        public int Aantal { get; set; }
    }
}
