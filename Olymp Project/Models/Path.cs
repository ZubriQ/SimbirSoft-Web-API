using System.ComponentModel.DataAnnotations;

namespace Olymp_Project.Models
{
    /// <summary>
    /// A hypothetical class to implement in the 3rd stage of the contest.
    /// </summary>
    public class Path
    {
        [Key]
        public long Id { get; set; }
        public long StartLocationId { get; set; }
        public long EndLocationId { get; set; }
        public double Weight { get; set; }
        public bool IsReversed { get; set; } = false;

        public virtual Location StartLocation { get; set; } = null!;
        public virtual Location EndLocation { get; set; } = null!;
    }
}
