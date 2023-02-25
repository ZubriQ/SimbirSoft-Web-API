namespace Olymp_Project.Authentication
{
    public interface IApiAuthenticationService
    {
        Task<Account?> AuthenticateAsync(string email, string password);
    }
}
