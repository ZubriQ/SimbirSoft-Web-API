using Olymp_Project.Responses;

namespace Olymp_Project.Services.Accounts
{
    public interface IAccountService
    {
        Task<IServiceResponse<Account>> GetAccountAsync(int? accountId);
        Task<IServiceResponse<ICollection<Account>>> GetAccountsAsync(AccountQuery query, Paging paging);
        Task<IServiceResponse<Account>> UpdateAccountAsync(int? accountId, AccountRequestDto request, string? email);
        Task<HttpStatusCode> RemoveAccountAsync(int? accountId, string? login);
    }
}
