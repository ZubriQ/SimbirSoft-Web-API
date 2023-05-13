namespace Olymp_Project.Dtos.ShortestPath
{
    /// <summary>
    /// A hypothetical dto class to implement in the 3rd stage of the contest.
    /// </summary>
    public class BellmanFordDto
    {
        public bool HasNegativeCycle { get; set; } = false;
        public double Distance { get; set; } = 0;
        public long[] Path { get; set; } = new long[0];
    }
}
