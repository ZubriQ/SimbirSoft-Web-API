using Geohash;
using System.Security.Cryptography;
using System.Text;

namespace Olymp_Project.Helpers.Geospatial
{
    public static class GeohashHelper
    {
        private static readonly Geohasher _hasher = new();
        private static readonly int _precision = 12;
        private static readonly MD5 _md5 = MD5.Create();

        public static string Encode(double latitude, double longitude)
        {
            return _hasher.Encode(latitude, longitude, _precision);
        }

        public static string EncodeV2(double latitude, double longitude)
        {
            string originalHash = Encode(latitude, longitude);
            byte[] hashBytes = Encoding.UTF8.GetBytes(originalHash);
            string base64Hash = Convert.ToBase64String(hashBytes);

            return base64Hash;
        }

        public static string EncodeV3(double latitude, double longitude)
        {
            var md5Hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(Encode(latitude, longitude)));
            byte[] resultHash = new byte[md5Hash.Length];
            for (int i = 0; i < md5Hash.Length; i++)
            {
                resultHash[i] = md5Hash[resultHash.Length - 1 - i];
            }

            return Convert.ToBase64String(resultHash);
        }
    }
}
