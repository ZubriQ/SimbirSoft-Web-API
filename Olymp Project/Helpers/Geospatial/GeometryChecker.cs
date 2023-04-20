using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Location = Olymp_Project.Models.Location;

namespace Olymp_Project.Helpers.Geospatial
{
    public class GeometryChecker
    {
        private GeometryFactory _geometryFactory;
        private Polygon _polygonShell;
        private LineString _polygonBoundary;

        public GeometryChecker(List<NpgsqlPoint> points)
        {
            _geometryFactory = new();

            var coordinates = points!.Select(p => new Coordinate(p.X, p.Y)).ToArray();
            var linearRing = new LinearRing(coordinates);
            _polygonShell = new Polygon(linearRing);
            _polygonBoundary = (LineString)_polygonShell.Boundary;
        }

        public bool IsLocationInsidePolygon(Location location)
        {
            return IsPointInsidePolygon(location.Longitude, location.Latitude);
        }

        private bool IsPointInsidePolygon(double x, double y)
        {
            var point = _geometryFactory.CreatePoint(new Coordinate(x, y));
            var distance = point.Distance(_polygonBoundary);
            var isInside = _polygonShell.Contains(point) || Math.Abs(distance) < 1e-6;
            return isInside;
        }
    }
}
