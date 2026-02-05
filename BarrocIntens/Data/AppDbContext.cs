using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using BCrypt.Net;

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
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteItem> QuoteItems { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<PlanningEmployee> PlanningEmployees { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }
        public DbSet<Deliverer> Deliverers { get; set; }


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
                },
                new Department
                {
                    Id = 5,
                    Name = "Management",
                    Description =
                      "Beheert alles"
                });
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Peter Jansen",
                    Email = "peter.jansen@bedrijf.nl",
                    PhoneNumber = "0612345678",
                    City = "Breda",
                    Password = BCrypt.Net.BCrypt.HashPassword("Welkom123"),
                    DepartmentId = 4,
                    
                    

                },

                new Employee
                {
                    Id = 2,
                    Name = "Lisa Vermeer",
                    Email = "lisa.vermeer@bedrijf.nl",
                    PhoneNumber = "0612285678",
                    City = "Breda",
                    Password = BCrypt.Net.BCrypt.HashPassword("Welkom123"),
                    DepartmentId = 4,
                },
                new Employee
                {
                    Id = 3,
                    Name = "Ahmed El Amrani",
                    Email = "ahmed.elamrani@bedrijf.nl",
                    PhoneNumber = "0612287128",
                    City = "Breda",
                    Password = BCrypt.Net.BCrypt.HashPassword("Welkom123"),
                    DepartmentId = 4,
                },

                new Employee
                {
                    Id = 4,
                    Name = "Root",
                    Email = "Root@root.nl",
                    PhoneNumber = "0675287128",
                    City = "Breda",
                    Password = BCrypt.Net.BCrypt.HashPassword("Root"),
                    DepartmentId = 5,
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
                    Category = "Storing",
                },
                new Planning
                {
                    Id = 2,
                    Date = new DateOnly(2025, 11, 11),
                    Plan = "Onderhoud warmtepompsystemen regio Rotterdam",
                    Description = "Installatie nieuwe zonnepanelen bij klant Jan de Vries",
                    Location = "Rotterdam",
                    Status = "niet gedaan",
                    Category = "Adviesbezoek",
                },
                new Planning
                {
                    Id = 3,
                    Date = new DateOnly(2025, 11, 12),
                    Plan = "Controle BKR-status en contractverlenging",
                    Description = "Installatie nieuwe zonnepanelen bij klant Jan de Vries",
                    Location = "Rotterdam",
                    Status = "niet gedaan",
                    Category = "Adviesbezoek",
                });
            modelBuilder.Entity<Deliverer>().HasData(
                new Deliverer
                {
                    Id = 1,
                    Name = "Wizzmie",
                    Email = "wizzmie@gmail.com",
                }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Wizzmie Coffee Machine",
                    Category = "IceCoffee",
                    Price = 249.99,
                    Stock = 50,
                    MinimumStock = 10,
                    DelivererId = 1,
                    //NotificationOutOfStock = false,
                    Image = "wizzmie.jpg",
                    IsMachine = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Amazon Coffee machine",
                    Category = "AllTypes",
                    Price = 1299.00,
                    Stock = 15,
                    MinimumStock = 5,
                    DelivererId = 1,
                    //NotificationOutOfStock = false,
                    Image = "arah.jpg",
                    IsMachine = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Arah Kopi",
                    Category = "AllTypes",
                    Price = 199.99,
                    Stock = 25,
                    MinimumStock = 8,
                    DelivererId = 1,
                    //NotificationOutOfStock = false,
                    Image = "arah.jpg",
                    IsMachine = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Pawon Luwak Machine",
                    Category = "AllTypes",
                    Price = 899.00,
                    Stock = 5,
                    MinimumStock = 3,
                    DelivererId = 1,
                    //NotificationOutOfStock = false,
                    Image = "wizzmie.jpg",
                    IsMachine = true
                },
                new Product
                {
                    Id = 5,
                    Name = "Aceh Arabica 1.000 kg",
                    Category = "Arabica",
                    Price = 159.00,
                    Stock = 5,
                    MinimumStock = 3,
                    DelivererId = 1,
                    //NotificationOutOfStock = false,
                    Image = "Gemini_Generated_Image_j5hdvhj5hdvhj5hd.png",
                    IsMachine = false
                }
            );
            modelBuilder.Entity<Material>().HasData(
                new Material { Id = 1, Name = "Rubber (10 mm)", Stock = 12, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 2, Name = "Rubber (14 mm)", Stock = 8, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 3, Name = "Slang", Stock = 5, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 4, Name = "Voeding (elektra)", Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", Stock = 2, MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 5, Name = "Ontkalker", Stock = 5, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 6, Name = "Waterfilter", Stock = 2, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 7, Name = "Reservoir sensor", Stock = 3, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 8, Name = "Druppelstop", Stock = 2, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 9, Name = "Elektrische pomp", Stock = 1, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 10, Name = "Tandwiel 110mm", Stock = 8, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 11, Name = "Tandwiel 70mm", Stock = 5, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 12, Name = "Maalmotor", Stock = 2, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 13, Name = "Zeef", Stock = 9, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 14, Name = "Reinigingstabletten", Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", Stock = 3, MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 15, Name = "Reinigingsborsteltjes", Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", Stock = 6, MinimumStock = 3, Quantity = 0, DelivererId = 1 },
                new Material { Id = 16, Name = "Ontkalkingspijp", Stock = 2, Image = "Gemini_Generated_Image_pdr2g3pdr2g3pdr2.png", MinimumStock = 3, Quantity = 0, DelivererId = 1 }
                ); 
            modelBuilder.Entity<ProductMaterial>().HasData(
                new ProductMaterial { Id = 1, ProductId = 1, MaterialId = 6 },  // Waterfilter
                new ProductMaterial { Id = 2, ProductId = 2, MaterialId = 5 },  // Ontkalker
                new ProductMaterial { Id = 3, ProductId = 2, MaterialId = 13 }, // Zeef
                new ProductMaterial { Id = 4, ProductId = 3, MaterialId = 10 }, // Tandwiel 110mm
                new ProductMaterial { Id = 5, ProductId = 2, MaterialId = 15 }  // Reinigingsborsteltjes
            );


        }
    }

}
