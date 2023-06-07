using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Accounts
{
    public interface IAccountService
    {
        Task<IResponse<Account>> GetAccountByIdAsync(int? accountId);

        Task<IResponse<ICollection<Account>>> GetAccountsBySearchParametersAsync(
            AccountQuery query, 
            Paging paging);

        Task<IResponse<Account>> InsertAccountAsync(AccountRequestDto account);

        Task<IResponse<Account>> UpdateAccountAsync(int? accountId, AccountRequestDto request);

        Task<HttpStatusCode> RemoveAccountAsync(int? accountId);
    }
}
