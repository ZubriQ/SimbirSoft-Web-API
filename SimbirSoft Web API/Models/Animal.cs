﻿using System.ComponentModel.DataAnnotations;

namespace SimbirSoft_Web_API.Models
{
    public partial class Animal
    {
        public Animal()
        {
            VisitedLocations = new HashSet<VisitedLocation>();
            Kinds = new HashSet<Kind>();
        }

        [Key]
        public long Id { get; set; }
        public float Weight { get; set; }
        public float Length { get; set; }
        public float Height { get; set; }
        public string Gender { get; set; } = null!;
        public string LifeStatus { get; set; } = "ALIVE";
        public int ChipperId { get; set; }
        public DateTime ChippingDateTime { get; set; } = DateTime.UtcNow;
        public long ChippingLocationId { get; set; }
        public DateTime? DeathDateTime { get; set; } = null;

        public virtual Account Chipper { get; set; } = null!;
        public virtual Location ChippingLocation { get; set; } = null!;

        public virtual ICollection<VisitedLocation> VisitedLocations { get; set; }
        public virtual ICollection<Kind> Kinds { get; set; }
    }
}
