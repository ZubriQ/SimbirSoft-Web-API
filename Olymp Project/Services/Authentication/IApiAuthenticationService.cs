namespace Olymp_Project.Authentication
{
    public interface IApiAuthenticationService
    {
        Task<Account?> AuthenticateAccountAsync(string email, string password);
    }
}
