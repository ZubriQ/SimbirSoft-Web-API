﻿namespace SimbirSoft_Web_API.Queries
{
    public class AnimalQuery
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
        public string? LifeStatus { get; set; }
        public string? Gender { get; set; }
    }
}
