using System.Net;

namespace Olymp_Project.Services.Registration
{
    public interface IRegistrationService
    {
        Task<ServiceResponse<Account>> RegisterAccountAsync(Account account);
    }
}
