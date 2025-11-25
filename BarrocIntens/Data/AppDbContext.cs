using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace BarrocIntens.Data
{
    internal class AppDbContext : DbContext
    {
        // These are the lists of the database
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Malfunction> Malfunctions { get; set; }
        public DbSet<Planning> Plannings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CostumerPlanning> CostumerPlannings { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            // building the database
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=;database=csd_BarrocIntens",
                ServerVersion.Parse("8.0.30"));
        }

        // models are required here
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment
                {
                    Id = 1,
                    CustomerId = 1,
                    EmployeeId = 1,
                    Date = new DateTime(2025, 11, 10, 14, 0, 0),
                    Location = "Amsterdam",
                    Notes = "Eerste consultatie",
                    Status = true,
                    Type = "Consult",
                    Time = 60
                },
                new Appointment
                {
                    Id = 2,
                    CustomerId = 2,
                    EmployeeId = 1,
                    Date = new DateTime(2025, 11, 12, 9, 30, 0),
                    Location = "Rotterdam",
                    Notes = "Vervolgafspraak",
                    Status = false,
                    Type = "Onderhoud",
                    Time = 45
                },
                new Appointment
                {
                    Id = 3,
                    CustomerId = 3,
                    EmployeeId = 2,
                    Date = new DateTime(2025, 11, 15, 16, 0, 0),
                    Location = "Utrecht",
                    Notes = "Nazorg controle",
                    Status = true,
                    Type = "Controle",
                    Time = 30
                });
            modelBuilder.Entity<Customer>().HasData(
    new Customer
    {
        Id = 1,
        Name = "Jan de Vries",
        Email = "jan.devries@example.com",
        PhoneNumber = "0612345678",
        City = "Amsterdam",
        BkrStatus = false
    },
    new Customer
    {
        Id = 2,
        Name = "Sophie van Dijk",
        Email = "sophie.vandijk@example.com",
        PhoneNumber = "0622334455",
        City = "Rotterdam",
        BkrStatus = true
    },
    new Customer
    {
        Id = 3,
        Name = "Ali Yılmaz",
        Email = "ali.yilmaz@example.com",
        PhoneNumber = "0688776655",
        City = "Utrecht",

        BkrStatus = false
    });
            modelBuilder.Entity<Bill>().HasData(
                new Bill
                {
                    Id = 1,
                    EmployeeId = 1,
                    CustomerId = 1,
                    TotalAmount = 250
                },
                new Bill
                {
                    Id = 2,
                    EmployeeId = 2,
                    CustomerId = 2,
                    TotalAmount = 480
                },
                new Bill
                {
                    Id = 3,
                    EmployeeId = 1,
                    CustomerId = 3,
                    TotalAmount = 320
                });


            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "Technische Dienst",
                    Description =
                      "Verantwoordelijk voor onderhoud, installaties en storingen."
                },
                new Department
                {
                    Id = 2,
                    Name = "Klantenservice",
                    Description = "Behandelt klantvragen, klachten en serviceverzoeken."
                },
                new Department
                {
                    Id = 3,
                    Name = "Administratie",
                    Description =
                      "Verwerkt facturen, betalingen en interne administratie."
                },
                new Department
                {
                    Id = 4,
                    Name = "Sales",
                    Description =
                      "Beheert klantrelaties en genereert nieuwe verkoopkansen."
                });
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Peter Jansen",
                    Email = "peter.jansen@bedrijf.nl",
                    Password = "Welkom123"
                },

                new Employee
                {
                    Id = 2,
                    Name = "Lisa Vermeer",
                    Email = "lisa.vermeer@bedrijf.nl",
                    Password = "Welkom123"
                },
                new Employee
                {
                    Id = 3,
                    Name = "Ahmed El Amrani",
                    Email = "ahmed.elamrani@bedrijf.nl",
                    Password = "Welkom123"
                },
                                new Employee
                                {
                                    Id = 4,
                                    Name = "Root",
                                    Email = "Root@root.nl",
                                    Password = "Root"
                                });

            modelBuilder.Entity<Malfunction>().HasData(
                new Malfunction
                {
                    Id = 1,
                    Date = new DateTime(2025, 10, 21),
                    Description = "Klant meldt dat de airconditioning niet meer koelt.",
                    status = false,
                    Action = "Afspraak ingepland voor inspectie.",
                    Urgency = "Hoog"
                },
                new Malfunction
                {
                    Id = 2,
                    Date = new DateTime(2025, 10, 25),
                    Description = "Waterlekkage gemeld bij warmtepomp.",
                    status = true,
                    Action = "Reparatie uitgevoerd, systeem getest.",
                    Urgency = "Middel"
                },
                new Malfunction
                {
                    Id = 3,
                    Date = new DateTime(2025, 11, 2),
                    Description =
                      "Storing aan zonnepanelen – geen opbrengst geregistreerd.",
                    status = false,
                    Action = "Monteur toegewezen voor diagnose.",
                    Urgency = "Hoog"
                });




            // Seeding data
            modelBuilder.Entity<Planning>().HasData(
                new Planning
                {
                    Id = 1,
                    Date = new DateOnly(2025, 11, 10),
                    Plan = "Installatie nieuwe zonnepanelen bij klant Jan de Vries",
                    Description = "Installatie nieuwe zonnepanelen bij klant Jan de Vries",
                    Location = "Rotterdam",
                    Status = "niet gedaan",
                },
                new Planning
                {
                    Id = 2,
                    Date = new DateOnly(2025, 11, 11),
                    Plan = "Onderhoud warmtepompsystemen regio Rotterdam",
                    Description = "Installatie nieuwe zonnepanelen bij klant Jan de Vries",
                    Location = "Rotterdam",
                    Status = "niet gedaan",
                },
                new Planning
                {
                    Id = 3,
                    Date = new DateOnly(2025, 11, 12),
                    Plan = "Controle BKR-status en contractverlenging",
                    Description = "Installatie nieuwe zonnepanelen bij klant Jan de Vries",
                    Location = "Rotterdam",
                    Status = "niet gedaan",
                });

            //modelBuilder.Entity<Planning>()
    //.HasMany(p => p.Customers)
    //.WithMany()
    //.UsingEntity<Dictionary<string, object>>(
    //    "PlanningCustomers",
    //    j => j.HasOne<Customer>().WithMany().HasForeignKey("CustomerId"),
    //    j => j.HasOne<Planning>().WithMany().HasForeignKey("PlanningId"),
    //    j =>
        //{
        //    j.HasKey("PlanningId", "CustomerId");
        //    j.ToTable("PlanningCustomers");

        //    // ✅ Join table seeding
        //    j.HasData(
        //        new { CustomerId = 1, PlanningId = 1 },
        //        new { CustomerId = 2, PlanningId = 2 },
        //        new { CustomerId = 3, PlanningId = 3 }
        //    );
        //});


            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Zonnepaneel Premium 400W",
                    Category = "Zonnepanelen",
                    Price = 249.99,
                    Stock = 50,
                    MinimumStock = 10,
                    Deliverer = "SolarTech BV",
                    NotificationOutOfStock = false,
                    Image = "logo.png"
                },
                new Product
                {
                    Id = 2,
                    Name = "Warmtepomp EcoHeat 3000",
                    Category = "Verwarming",
                    Price = 1299.00,
                    Stock = 15,
                    MinimumStock = 5,
                    Deliverer = "GreenEnergy Supply",
                    NotificationOutOfStock = false,
                    Image = "logo.png"
                },
                new Product
                {
                    Id = 3,
                    Name = "Slimme thermostaat ComfortPlus",
                    Category = "Smart Home",
                    Price = 199.99,
                    Stock = 25,
                    MinimumStock = 8,
                    Deliverer = "SmartHome Solutions",
                    NotificationOutOfStock = false,
                    Image = "logo.png"
                },
                new Product
                {
                    Id = 4,
                    Name = "Omvormer SolarLink 5kW",
                    Category = "Zonnepanelen",
                    Price = 899.00,
                    Stock = 5,
                    MinimumStock = 3,
                    Deliverer = "EnergyPro NV",
                    NotificationOutOfStock = true,
                    Image = "logo.png"
                });
        }
    }

}
