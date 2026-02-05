using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class OfferItem
    {
        public int Id { get; set; }
        public int DelivererId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }

    }
}
