using Olymp_Project.Dtos.LocationPath;
using Path = Olymp_Project.Models.Path;

namespace Olymp_Project.Helpers.Geospatial
{
    /// <summary>
    /// A hypothetical class to implement in the 3rd stage of the contest.
    /// </summary>
    public class WeightedPathFinder
    {
        private Dictionary<Location, double> _distances = new();
        private Dictionary<Location, Location> _track = new();
        private List<Location> _visitedNodes = new();
        private List<Location> _unvisitedNodes = new();

        /// <summary>
        /// Uses Dijkstra algorithm to find the shortest path.
        /// </summary>
        /// <param name="paths">Edges.</param>
        /// <param name="startNode">Start node.</param>
        /// <param name="endNode">End node.</param>
        /// <returns>The shortest path and distance.</returns>
        public DijkstraDto FindShortestPath(
            List<Path> paths, 
            Location startNode, 
            Location endNode)
        {
            InitializeUnvisitedNodes(paths);
            InitializeDistances(startNode);
            Traverse();

            var path = GetPathFromStartToEnd(startNode, endNode);
            if (path.Count() == 0)
            {
                return new DijkstraDto();
            }

            DijkstraDto dto = new DijkstraDto()
            {
                Path = path.Select(p => p.Id).ToArray(),
                Distance = _distances[endNode],
            };
            return dto;
        }

        #region Initialization

        private void InitializeUnvisitedNodes(List<Path> paths)
        {
            foreach (var path in paths)
            {
                if (!_unvisitedNodes.Contains(path.StartLocation))
                {
                    _unvisitedNodes.Add(path.StartLocation);
                }
                if (!_unvisitedNodes.Contains(path.EndLocation))
                {
                    _unvisitedNodes.Add(path.EndLocation);
                }
            }
        }

        private void InitializeDistances(Location start)
        {
            foreach (var node in _unvisitedNodes)
            {
                if (node.Id == start.Id)
                {
                    _distances[node] = 0;
                }
                else
                {
                    _distances[node] = double.PositiveInfinity;
                }
            }
        }

        #endregion

        private void Traverse()
        {
            while (_unvisitedNodes.Count > 0)
            {
                var currentNode = _unvisitedNodes.OrderBy(location => _distances[location]).First();

                UpdateDistancesAndTrack(currentNode);
                _visitedNodes.Add(currentNode);
                _unvisitedNodes.Remove(currentNode);
            }
        }

        private void UpdateDistancesAndTrack(Location currentNode)
        {
            foreach (var path in currentNode.PathsFrom.OrderBy(p => p.Weight))
            {
                double nextWeight = _distances[currentNode] + path.Weight;
                if (nextWeight < _distances[path.EndLocation])
                {
                    _distances[path.EndLocation] = nextWeight;
                    _track[path.EndLocation] = currentNode;
                }
            }
        }

        private IEnumerable<Location> GetPathFromStartToEnd(Location start, Location end)
        {
            if (!_track.ContainsKey(end))
            {
                return Enumerable.Empty<Location>();
            }

            var shortestPath = new List<Location>();
            var current = end;
            while (current != start)
            {
                shortestPath.Add(current);
                current = _track[current];
            }
            shortestPath.Add(start);
            shortestPath.Reverse();

            return shortestPath;
        }
    }
}
