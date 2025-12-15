
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Planning
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="datum is verplicht")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage ="Voer een gebeurtenis in")]
        public string Plan { get; set; }

        //[Required]
        // betrokken personen
        //public List<Customer> Customers { get; set; }
        
        // koppel tabel maken?
        //public int EmployeeId { get; set; }
        //public Employee Employee { get; set; }


        [Required]
        public string Status { get; set; }

        [Required]
        [MaxLength(180)]
        public string Location { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Category { get; set;}

        // CRUD operations for Planning

        // Update
        public void Update(int planningId, DateOnly date, string plan, string status, string location, string description)
        {
            using (var db = new AppDbContext())
            {
                var planning = db.Plannings.FirstOrDefault(p => p.Id == planningId);
                if (planning != null)
                {
                    planning.Date = date;
                    planning.Plan = plan;
                    planning.Status = status;
                    planning.Location = location;
                    planning.Description = description;
                    db.SaveChanges();
                }
            }
        }

        // Delete
        public void Delete(int planningId)
        {
            using (var db = new AppDbContext())
            {
                var planning = db.Plannings.FirstOrDefault(p => p.Id == planningId);
                if (planning != null)
                {
                    db.Plannings.Remove(planning);
                    db.SaveChanges();
                }
            }
        }
    }
}
