using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    public class Deliverer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [AllowNull]
        public string Email { get; set; }
        [AllowNull]
        public ProductDeliverer ProductDeliverer { get; set; }
        [AllowNull]
        public MaterialDeliverer MaterialDeliverer { get; set; }
    }
}
