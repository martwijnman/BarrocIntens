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
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage ="Voer een naam in")]
        [MaxLength(180)]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Voer een naam in")]
        [MaxLength(180)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Voer een naam in")]
        [MaxLength(180)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Voer een naam in")]
        [MaxLength(180)]
        public string City { get; set; }

        [Required(ErrorMessage = "Voer een monteur in")]
        public int MalfunctionId { get; set; }
        public Malfunction Malfunction { get; set; }
        // contractduur
        // financiele status
        public bool BkrStatus { get; set; } // nullable
                                            // CRUD operations for Department

        // Update
        public void Update(int customerId, string name, string email, string phoneNumber, string city, int malfunctionId, bool bkrStatus)
        {
            using (var db = new AppDbContext())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);
                if (customer != null)
                {
                    customer.Name = name;
                    customer.Email = email;
                    customer.PhoneNumber = phoneNumber;
                    customer.City = city;
                    customer.MalfunctionId = malfunctionId;
                    customer.BkrStatus = bkrStatus;
                    db.SaveChanges();
                }
            }
        }

        // Delete
        public void Delete(int customerId)
        {
            using (var db = new AppDbContext())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);
                if (customer != null)
                {
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                }
            }
        }
    }
}
