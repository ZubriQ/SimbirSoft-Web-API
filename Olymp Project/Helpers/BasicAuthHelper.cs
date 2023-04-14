using System.Text;

namespace Olymp_Project.Helpers
{
    public static class BasicAuthHelper
    {
        public static (string? email, string? password) ExtractBasicAuthCredentials(string? headerParameter)
        {
            if (string.IsNullOrWhiteSpace(headerParameter))
            {
                return (null, null);
            }

            var credentialBytes = Convert.FromBase64String(headerParameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var email = credentials[0];
            var password = credentials[1];

            return (email, password);
        }
    }
}
