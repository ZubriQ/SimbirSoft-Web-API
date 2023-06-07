namespace SimbirSoft_Web_API.Authentication
{
    public interface IApiAuthenticationService
    {
        Task<Account?> AuthenticateAccountAsync(string email, string password);
    }
}
