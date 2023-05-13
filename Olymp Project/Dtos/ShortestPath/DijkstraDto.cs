namespace Olymp_Project.Dtos.LocationPath
{
    /// <summary>
    /// A hypothetical dto class to implement in the 3rd stage of the contest.
    /// </summary>
    public class DijkstraDto
    {
        public double Distance { get; set; }
        public long[] Path { get; set; } = null!;
    }
}
