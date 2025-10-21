using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class AppDbContext : DbContext
    {
        // These are the lists of the database
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Malfunction> Malfunctions { get; set; }
        public DbSet<Planning> Plannings { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // building the database
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=;database=csd_BarrocIntens",
                ServerVersion.Parse("8.0.30")
            );
        }
        // models are required here

    }
}
