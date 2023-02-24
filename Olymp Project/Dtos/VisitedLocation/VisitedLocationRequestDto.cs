namespace Olymp_Project.Dtos.VisitedLocation
{
    public class VisitedLocationRequestDto
    {
        /// <summary>
        /// Visited location that we need to update.
        /// </summary>
        public long? VisitedLocationId { get; set; }

        /// <summary>
        /// New location.
        /// </summary>
        public long? LocationId { get; set; }
    }
}
