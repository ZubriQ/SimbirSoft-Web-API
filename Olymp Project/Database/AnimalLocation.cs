namespace Olymp_Project.Database
{
    public class AnimalLocation
    {
        public long Id { get; set; }
        public DateTime DateTimeOfVisitLocationPoint { get; set; }
        public long LocationPointId { get; set; }

        public AnimalLocation(long id, DateTime dateTimeOfVisitLocationPoint, long locationPointId)
        {
            Id = id;
            DateTimeOfVisitLocationPoint = dateTimeOfVisitLocationPoint;
            LocationPointId = locationPointId;
        }
    }
}
