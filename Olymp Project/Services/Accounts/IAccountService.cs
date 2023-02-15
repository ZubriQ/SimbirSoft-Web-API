using System.Net;

namespace Olymp_Project.Services.Accounts
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
