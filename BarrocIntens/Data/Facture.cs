using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Facture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int QuoteId { get; set; }
        public Quote Quote { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        public DateOnly StartDate = DateOnly.FromDateTime(DateTime.Now).AddDays(5);
        public DateOnly EndDate = DateOnly.FromDateTime(DateTime.Now).AddDays(370);
    }
}
