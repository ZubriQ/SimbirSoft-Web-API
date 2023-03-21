namespace Olymp_Project.Models
{
    public partial class ChipizationDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public ChipizationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<Animal> Animals { get; set; } = null!;
        public DbSet<Kind> Kinds { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<VisitedLocation> VisitedLocations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
        }
    }
}
