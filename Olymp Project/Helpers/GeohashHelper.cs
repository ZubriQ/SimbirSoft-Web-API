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
            string originalHash = Encode(latitude, longitude);
            byte[] hashBytes = Encoding.ASCII.GetBytes(originalHash);
            string base64Hash = Convert.ToBase64String(hashBytes);

            return base64Hash;
        }

        private static readonly byte[] _xorKey = Encoding.ASCII.GetBytes("qwerty123");

        public static string EncodeV3(double latitude, double longitude)
        {
            string base64Hash = EncodeV2(latitude, longitude);
            byte[] hashBytes = Encoding.ASCII.GetBytes(base64Hash);

            byte[] encryptedBytes = new byte[hashBytes.Length];
            for (int i = 0; i < hashBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(hashBytes[i] ^ _xorKey[i % _xorKey.Length]);
            }

            string encryptedHash = Convert.ToBase64String(encryptedBytes);
            return encryptedHash;
        }

        //private static readonly byte[] _xorKey = Encoding.ASCII.GetBytes("qwerty123");

        //public static string EncodeV3(double latitude, double longitude)
        //{
        //    string base64Hash = EncodeV2(latitude, longitude);
        //    byte[] hashBytes = Encoding.ASCII.GetBytes(base64Hash);

        //    byte[] encryptedBytes = new byte[hashBytes.Length];
        //    for (int i = 0; i < hashBytes.Length; i++)
        //    {
        //        encryptedBytes[i] = (byte)(hashBytes[i] ^ _xorKey[i % _xorKey.Length]);
        //    }

        //    string encryptedHash = Convert.ToBase64String(encryptedBytes);
        //    return encryptedHash;
        //}
    }
}
