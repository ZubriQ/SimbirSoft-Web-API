using System.Net;

namespace Olymp_Project.Services.Registration
{
    public interface IRegistrationService
    {
        Task<(HttpStatusCode, Account?)> RegisterAccountAsync(Account account);
    }
}
