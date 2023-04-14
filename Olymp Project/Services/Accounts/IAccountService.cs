using Olymp_Project.Responses;
using System.Security.Claims;

namespace Olymp_Project.Services.Accounts
{
    public interface IAccountService
    {
        Task<IServiceResponse<Account>> GetAccountByIdAsync(int? accountId);
        IServiceResponse<ICollection<Account>> GetAccounts(AccountQuery query, Paging paging);
        Task<IServiceResponse<Account>> InsertAccountAsync(AccountRequestDto account);
        Task<IServiceResponse<Account>> UpdateAccountAsync(int? accountId, AccountRequestDto request);
        Task<HttpStatusCode> RemoveAccountAsync(int? accountId);
    }
}
