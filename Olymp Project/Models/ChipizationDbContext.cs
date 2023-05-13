using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace Olymp_Project.Models
{
    public partial class ChipizationDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public ChipizationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Animal> Animals { get; set; } = null!;
        public DbSet<Kind> Kinds { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<VisitedLocation> VisitedLocations { get; set; } = null!;
        public DbSet<Area> Areas { get; set; } = null!;
        public DbSet<Path> Paths { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Path

            modelBuilder.Entity<Path>().HasKey(p => p.Id);
                //.HasKey(p => new { p.StartLocationId, p.EndLocationId });

            modelBuilder.Entity<Path>()
                .HasOne(p => p.StartLocation)
                .WithMany(l => l.PathsFrom)
                //.HasForeignKey(p => p.StartLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Path>()
                .HasOne(p => p.EndLocation)
                .WithMany(l => l.PathsTo)
                //.HasForeignKey(p => p.EndLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Add initial account data

            modelBuilder.Entity<Account>().HasData(
                new Account()
                {
                    Id = 1,
                    FirstName = "adminFirstName",
                    LastName = "adminLastName",
                    Email = "admin@simbirsoft.com",
                    Password = "qwerty123",
                    Role = "ADMIN"
                },
                new Account()
                {
                    Id = 2,
                    FirstName = "chipperFirstName",
                    LastName = "chipperLastName",
                    Email = "chipper@simbirsoft.com",
                    Password = "qwerty123",
                    Role = "CHIPPER"
                },
                new Account()
                {
                    Id = 3,
                    FirstName = "userFirstName",
                    LastName = "userLastName",
                    Email = "user@simbirsoft.com",
                    Password = "qwerty123",
                    Role = "USER"
                });

            #endregion

            #region Hypothetical SuperChipper initial test data

            modelBuilder.Entity<Account>().HasData(
                new Account()
                {
                    Id = 4,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "4",
                    Password = "4",
                    Role = "SUPERCHIPPER"
                });

            #endregion

            modelBuilder.HasDbFunction(typeof(ChipizationDbContext)
                .GetMethod(nameof(GetAllPaths))!)
                .HasName("GetAllPaths");

            base.OnModelCreating(modelBuilder);
        }

        public IQueryable<Path> GetAllPaths() =>
            Paths.FromSqlInterpolated($"SELECT * FROM GetAllPaths()");
    }
}
