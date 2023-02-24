namespace Olymp_Project.Dtos.VisitedLocation
{
    public class VisitedLocationRequestDto
    {
        /// <summary>
        /// Old location.
        /// </summary>
        public long? VisitedLocationId { get; set; }

        /// <summary>
        /// Newly visited location.
        /// </summary>
        public long? LocationId { get; set; }
    }
}
