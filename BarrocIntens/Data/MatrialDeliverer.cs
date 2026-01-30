using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    public class MatrialDeliverer
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int MatrialId { get; set; }
        public Matrial Matrial { get; set;  }

        [Required]
        public int DelivererId { get; set; }
        public Deliverer Deliverer { get; set;  }
    }
}
