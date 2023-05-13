using System.ComponentModel.DataAnnotations;

namespace Olymp_Project.Models
{
    public partial class Location
    {
        public Location()
        {
            Animals = new HashSet<Animal>();
            VisitedLocations = new HashSet<VisitedLocation>();
            PathsFrom = new HashSet<Path>();
            PathsTo = new HashSet<Path>();
        }

        [Key]
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<VisitedLocation> VisitedLocations { get; set; }

        public virtual ICollection<Path> PathsFrom { get; set; }
        public virtual ICollection<Path> PathsTo { get; set; }
    }
}
