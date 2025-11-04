using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Bill
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Voeg een klant toe")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required(ErrorMessage = "Voeg een klant toe")]
        public int CostumerId { get; set; }
        public Customer Customer { get; set; }

        [Required(ErrorMessage = "Voeg een bedrag toe")]
        public int TotalAmount { get; set; }

        // CRUD operations for Bill

        // Update
        public void Update(int billId, int employeeId, int costumerIdemail, int totalAmount)
        {
            using (var db = new AppDbContext())
            {
                var bill = db.Bills.FirstOrDefault(b => b.Id == billId);
                if (bill != null)
                {
                    bill.EmployeeId = employeeId;
                    bill.CostumerId = costumerIdemail;
                    bill.TotalAmount = totalAmount;
                    db.SaveChanges();
                }
            }
        }

        // Delete
        public void Delete(int billId)
        {
            using (var db = new AppDbContext())
            {
                var bill = db.Bills.FirstOrDefault(e => e.Id == billId);
                if (bill != null)
                {
                    db.Bills.Remove(bill);
                    db.SaveChanges();
                }
            }
        }

    }
}
