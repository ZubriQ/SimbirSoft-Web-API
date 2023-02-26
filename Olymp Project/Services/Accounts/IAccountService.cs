using Olymp_Project.Responses;
using System.Net;

namespace Olymp_Project.Services.Accounts
{
    public interface IAccountService
    {
        Task<IServiceResponse<Account>> GetAccountAsync(int? id);
        Task<IServiceResponse<ICollection<Account>>> GetAccountsAsync(AccountQuery query, Paging paging);
        Task<IServiceResponse<Account>> UpdateAccountAsync(int? id, AccountRequestDto request);
        Task<HttpStatusCode> RemoveAccountAsync(int? id, string? login);
    }
}
