using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    public class MaterialDeliverer
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int MaterialId { get; set; }
        public Material Material { get; set;  }

        [Required]
        public int DelivererId { get; set; }
        public Deliverer Deliverer { get; set;  }
    }
}
