using Geohash;
using System.Text;

namespace Olymp_Project.Helpers
{
    public static class GeohashHelper
    {
        private static readonly Geohasher _hasher = new();
        private static readonly int _precision = 12;

        public static string Encode(double latitude, double longitude)
        {
            return _hasher.Encode(latitude, longitude, _precision);
        }

        public static string EncodeV2(double latitude, double longitude)
        {
            string originalHash = _hasher.Encode(latitude, longitude, _precision);
            byte[] hashBytes = Encoding.ASCII.GetBytes(originalHash);
            string base64Hash = Convert.ToBase64String(hashBytes);

            return base64Hash;
        }

        public static string EncodeV3(double latitude, double longitude)
        {
            return "not implemented";
        }
    }
}
