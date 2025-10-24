using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Customer
    {
        [Required]
        public int Id { get; set; }
        
        [Required(ErrorMessage ="Voer een naam in")]
        [MaxLength(180)]
        public string name { get; set; }
        
        [Required(ErrorMessage = "Voer een naam in")]
        [MaxLength(180)]
        public string email { get; set; }

        [Required(ErrorMessage = "Voer een naam in")]
        [MaxLength(180)]
        public string phoneNumber { get; set; }

        [Required(ErrorMessage = "Voer een naam in")]
        [MaxLength(180)]
        public string city { get; set; }

        [Required(ErrorMessage = "Voer een monteur in")]
        public int malfunctionId { get; set; }
        public Malfunction Malfunction { get; set; }
        // contractduur
        // financiele status
        public bool bkrStatus { get; set; } // nullable
    }
}
