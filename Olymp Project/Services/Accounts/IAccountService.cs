using Olymp_Project.Responses;

namespace Olymp_Project.Services.Accounts
{
    public interface IAccountService
    {
        Task<IServiceResponse<Account>> GetAccountByIdAsync(int? accountId);
        IServiceResponse<ICollection<Account>> GetAccounts(AccountQuery query, Paging paging);
        Task<IServiceResponse<Account>> UpdateAccountAsync(int? accountId, AccountRequestDto request, string? email);
        Task<HttpStatusCode> RemoveAccountAsync(int? accountId, string? login);
    }
}
