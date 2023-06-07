using System.ComponentModel.DataAnnotations;

namespace SimbirSoft_Web_API.Models
{
    public partial class Location
    {
        public Location()
        {
            Animals = new HashSet<Animal>();
            VisitedLocations = new HashSet<VisitedLocation>();
        }

        [Key]
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<VisitedLocation> VisitedLocations { get; set; }
    }
}
