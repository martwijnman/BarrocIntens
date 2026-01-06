using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    class Deliverer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductDeliverer ProductDeliverer { get; set; }
    }
}
