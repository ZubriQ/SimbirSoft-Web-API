using Microsoft.AspNetCore.Authentication;

namespace Olymp_Project.Services.Authentication
{
    public static class ApiAuthentication
    {
        public static async Task<bool> IsAuthorizationValid(HttpRequest request, HttpContext httpContext)
        {
            string authHeader = request.Headers["Authorization"];
            if (authHeader is not null && authHeader.StartsWith("Basic"))
            {
                var authResult = await httpContext.AuthenticateAsync(ApiAuthenticationScheme.Name);
                if (!authResult.Succeeded)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
