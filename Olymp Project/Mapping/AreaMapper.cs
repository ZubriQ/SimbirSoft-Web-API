using NetTopologySuite.Geometries;
using NpgsqlTypes;

namespace Olymp_Project.Mapping
{
    public class AreaMapper
    {
        private GeometryFactory _geometryFactory = new();

        public Polygon ToPolygonGeometry(NpgsqlPolygon npgsqlPolygon)
        {
            var coordinates = new Coordinate[npgsqlPolygon.Count + 1];
            for (int i = 0; i < npgsqlPolygon.Count; i++)
            {
                coordinates[i] = new Coordinate(npgsqlPolygon[i].X, npgsqlPolygon[i].Y);
            }
            coordinates[npgsqlPolygon.Count] = coordinates[0];

            return _geometryFactory.CreatePolygon(coordinates);
        }

        public Area ToArea(AreaRequestDto request)
        {
            var npgsqlPoints = request.AreaPoints!
                .Select(ToNpgsqlPoint)
                .ToArray();
            var npgsqlPolygon = new NpgsqlPolygon(npgsqlPoints);

            return new Area
            {
                Name = request.Name!,
                Points = npgsqlPolygon
            };
        }

        public static AreaPointsDto ToAreaPointsDto(NpgsqlPoint point)
        {
            return new AreaPointsDto
            {
                Longitude = point.X,
                Latitude = point.Y
            };
        }

        private static NpgsqlPoint ToNpgsqlPoint(AreaPointsDto point)
        {
            return new NpgsqlPoint(point.Longitude!.Value, point.Latitude!.Value);
        }
    }
}
