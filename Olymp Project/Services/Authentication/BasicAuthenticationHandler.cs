using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Olymp_Project.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IApiAuthenticationService _userService;

        public BasicAuthenticationHandler(
            IApiAuthenticationService userService,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header not found.");
            }

            var authenticationHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authenticationHeader.Scheme != "Basic")
            {
                return AuthenticateResult.Fail("Authentication scheme should be Basic.");
            }

            if (await AuthenticateAccountAsync(authenticationHeader.Parameter!) is not Account account)
            {
                return AuthenticateResult.Fail("Invalid email or password.");
            }

            return GetAuthenticateResult(account);
        }

        private async Task<Account?> AuthenticateAccountAsync(string? headerParameter)
        {
            var credentialBytes = Convert.FromBase64String(headerParameter!);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var email = credentials[0];
            var password = credentials[1];

            return await _userService.AuthenticateAccountAsync(email, password);
        }

        private AuthenticateResult GetAuthenticateResult(Account account)
        {
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Email),
                new Claim(ClaimTypes.Role, account.Role)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
