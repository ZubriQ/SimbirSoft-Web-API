using Olymp_Project.Dtos.Location;
using System.Net;

namespace Olymp_Project.Services
{
    public interface IAccountService
    {
        Task<(HttpStatusCode, Account?)> AddAccountAsync(Account account);
        Task<Account?> GetAccountAsync(int id);
        Task<IEnumerable<Account>> GetAccountsAsync(AccountQuery query, Paging paging);
        Task<Account?> UpdateAccountAsync(int id, AccountRequestDto account);
        Task<HttpStatusCode> DeleteAccountAsync(int id);
    }
}
