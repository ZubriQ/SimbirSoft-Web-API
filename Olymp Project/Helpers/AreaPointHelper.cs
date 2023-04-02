using NpgsqlTypes;

namespace Olymp_Project.Helpers
{
    public static class AreaPointHelper
    {
        public static NpgsqlPoint ToNpgsqlPoint(AreaPointsDto point)
        {
            return new NpgsqlPoint(point.Longitude!.Value, point.Latitude!.Value);
        }

        public static AreaPointsDto ToAreaPointsDto(NpgsqlPoint point)
        {
            return new AreaPointsDto
            {
                Longitude = point.X,
                Latitude = point.Y
            };
        }
    }
}
