using Microsoft.AspNetCore.Authentication;

namespace Olymp_Project.Services.Authentication
{
    public static class ApiAuthentication
    {

        // TODO: Replace with Middleware (remove this class)
        public static async Task<bool> IsAuthorizationValid(HttpRequest request, HttpContext httpContext)
        {
            string header = request.Headers["Authorization"];
            if (header is not null && header.StartsWith("Basic"))
            {
                var authResult = await httpContext.AuthenticateAsync(Constants.BasicAuthScheme);
                return authResult.Succeeded;
            }
            return true;
        }
    }
}
