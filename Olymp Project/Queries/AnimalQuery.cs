namespace Olymp_Project.Queries
{
    public class AnimalQuery
    {
        public DateTime? StartDateTime { get; set; } // TODO: try to optimize
        public DateTime? EndDateTime { get; set; }
        public int? ChipperId { get; set; }
        public long? ChippingLocationId { get; set; }
        public string? LifeStatus { get; set; }
        public string? Gender { get; set; }
    }
}
