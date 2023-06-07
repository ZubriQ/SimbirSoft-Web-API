using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Location = SimbirSoft_Web_API.Models.Location;

namespace SimbirSoft_Web_API.Helpers.Geospatial;

public class GeometryChecker
{
    private readonly GeometryFactory _geometryFactory;
    private readonly Polygon _polygonShell;
    private readonly LineString _polygonBoundary;

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
        return _polygonShell.Contains(point) || Math.Abs(distance) < 1e-6;
    }
}
