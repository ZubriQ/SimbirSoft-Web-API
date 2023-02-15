using System;
using System.Collections.Generic;

namespace Olymp_Project.Models
{
    public partial class Animal
    {
        public Animal()
        {
            VisitedLocations = new HashSet<VisitedLocation>();
            Kinds = new HashSet<Kind>();
        }

        public long Id { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = null!;
        public string LifeStatus { get; set; } = "ALIVE";
        public int ChipperId { get; set; }
        public DateTime ChippingDateTime { get; set; } = DateTime.Now;
        public long ChippingLocationId { get; set; }
        public DateTime? DeathDateTime { get; set; } = null;

        public virtual Account Chipper { get; set; } = null!;
        public virtual Location ChippingLocation { get; set; } = null!;
        public virtual ICollection<VisitedLocation> VisitedLocations { get; set; }

        public virtual ICollection<Kind> Kinds { get; set; }
    }
}
