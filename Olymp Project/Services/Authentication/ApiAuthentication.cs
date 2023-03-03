﻿using Microsoft.AspNetCore.Authentication;

namespace Olymp_Project.Services.Authentication
{
    public static class ApiAuthentication
    {
        // TODO: Somehow do it in the Middleware layer?
        public static async Task<bool> IsAuthorizationValid(HttpRequest request, HttpContext httpContext)
        {
            string header = request.Headers["Authorization"];
            if (header is not null && header.StartsWith("Basic"))
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
