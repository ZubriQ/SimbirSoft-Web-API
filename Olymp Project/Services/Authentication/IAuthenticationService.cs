namespace Olymp_Project.Authentication
{
    public interface IAuthenticationService
    {
        Task<Account?> AuthenticateAsync(string email, string password);
    }
}
