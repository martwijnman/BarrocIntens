using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Department
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage ="Voer afdelingnaam in")]
        [MaxLength(180)]
        public string name { get; set; }

        [Required]
        public string description { get; set; }
    }
}
