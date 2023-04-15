using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Olymp_Project.Helpers;

namespace Olymp_Project.Mapping
{
    public class AreaMapper
    {
        public Area ToArea(AreaRequestDto request)
        {
            var npgsqlPoints = request.AreaPoints!
                .Select(AreaPointHelper.ToNpgsqlPoint)
                .ToArray();
            var npgsqlPolygon = new NpgsqlPolygon(npgsqlPoints);

            return new Area
            {
                Name = request.Name!,
                Points = npgsqlPolygon
            };
        }

        public Polygon ToPolygonGeometry(NpgsqlPolygon npgsqlPolygon)
        {
            var coordinates = new Coordinate[npgsqlPolygon.Count + 1];
            for (int i = 0; i < npgsqlPolygon.Count; i++)
            {
                coordinates[i] = new Coordinate(npgsqlPolygon[i].X, npgsqlPolygon[i].Y);
            }
            coordinates[npgsqlPolygon.Count] = coordinates[0];

            var geometryFactory = new GeometryFactory();
            return geometryFactory.CreatePolygon(coordinates);
        }
    }
}
