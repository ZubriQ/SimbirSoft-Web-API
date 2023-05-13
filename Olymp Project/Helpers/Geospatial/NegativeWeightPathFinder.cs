using Olymp_Project.Dtos.ShortestPath;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Helpers.Geospatial
{
    /// <summary>
    /// A hypothetical class to implement in the 3rd stage of the contest.
    /// </summary>
    public class NegativeWeightPathFinder
    {
        Dictionary<Location, double> _distances = new();
        Dictionary<Location, Location> _track = new();

        /// <summary>
        /// Uses Bellman-Ford algorithm to find the shortest path.
        /// </summary>
        /// <param name="paths">Edges.</param>
        /// <param name="startNode">Start node.</param>
        /// <param name="endNode">End node.</param>
        /// <returns>Information about whether there are cycles in the tree,
        /// the shortest path, and the distance.</returns>
        public BellmanFordDto FindShortestPath(
            List<Path> paths, 
            Location startNode, 
            Location endNode)
        {
            InitializeDistances(paths, startNode);
            RelaxEdges(paths);
            if (NegativeCycleExists(paths))
            {
                return new BellmanFordDto() { HasNegativeCycle = true };
            }

            return GetPathFromStartToEnd(startNode, endNode);
        }

        private void InitializeDistances(List<Path> paths, Location start)
        {
            foreach (var path in paths)
            {
                _distances[path.StartLocation] = double.PositiveInfinity;
                _distances[path.EndLocation] = double.PositiveInfinity;
            }
            _distances[start] = 0;
        }

        private void RelaxEdges(List<Path> paths)
        {
            for (int i = 0; i < paths.Count - 1; i++)
            {
                foreach (var path in paths)
                {
                    RelaxEdge(path);
                }
            }
        }

        private void RelaxEdge(Path path)
        {
            double newDistance = _distances[path.StartLocation] + path.Weight;
            if (newDistance < _distances[path.EndLocation])
            {
                _distances[path.EndLocation] = newDistance;
                _track[path.EndLocation] = path.StartLocation;
            }
        }

        private bool NegativeCycleExists(List<Path> paths)
        {
            foreach (var path in paths)
            {
                double newDistance = _distances[path.StartLocation] + path.Weight;
                if (newDistance < _distances[path.EndLocation])
                {
                    return true;
                }
            }

            return false;
        }

        private BellmanFordDto GetPathFromStartToEnd(Location start, Location end)
        {
            List<long> pathList = new List<long>();
            Location current = end;
            while (_track.ContainsKey(current))
            {
                pathList.Add(current.Id);
                current = _track[current];
            }
            pathList.Add(start.Id);
            pathList.Reverse();

            return new BellmanFordDto
            {
                Distance = _distances[end],
                Path = pathList.ToArray()
            };
        }
    }
}
