using System.ComponentModel.DataAnnotations;

namespace Olymp_Project.Models
{
    public partial class VisitedLocation
    {
        [Key]
        public long Id { get; set; }
        public long LocationId { get; set; }
        public long AnimalId { get; set; }
        public DateTime VisitDateTime { get; set; }

        public virtual Animal Animal { get; set; } = null!;
        public virtual Location Location { get; set; } = null!;
    }
}
