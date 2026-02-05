using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Department
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage ="Voer afdelingnaam in")]
        [MaxLength(180)]
        public string Name { get; set; }


        [Required]
        public string Description { get; set; }

        public string? Icon { get; set; }

        // CRUD operations for Department

        // Update
        public void Update(int departmentId, string name, string description)
        {
            using (var db = new AppDbContext())
            {
                var department = db.Departments.FirstOrDefault(b => b.Id == departmentId);
                if (department != null)
                {
                    department.Name = name;
                    department.Description = description;
                    db.SaveChanges();
                }
            }
        }

        // Delete
        public void Delete(int departmentId)
        {
            using (var db = new AppDbContext())
            {
                var department = db.Departments.FirstOrDefault(d => d.Id == departmentId);
                if (department != null)
                {
                    db.Departments.Remove(department);
                    db.SaveChanges();
                }
            }
        }

    }
}
