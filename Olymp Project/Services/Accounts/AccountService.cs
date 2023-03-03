using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Accounts
{
    public partial class AccountService : IAccountService
    {
        private readonly ChipizationDbContext _db;

        public AccountService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<IServiceResponse<Account>> GetAccountAsync(int? id)
        {
            if (!IdValidator.IsValid(id))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id) is not Account account)
            {
                return new ServiceResponse<Account>(HttpStatusCode.NotFound, null!);
            }

            return new ServiceResponse<Account>(HttpStatusCode.OK, account);
        }

        public async Task<IServiceResponse<ICollection<Account>>> GetAccountsAsync(
            AccountQuery query, Paging paging)
        {
            if (!PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            try
            {
                var accounts = GetFilteredAccounts(query, paging);
                return new CollectionServiceResponse<Account>(HttpStatusCode.OK, accounts);
            }
            catch (Exception)
            {
                return new CollectionServiceResponse<Account>();
            }
        }

        // TODO: Make this layer as a Service, and move the method into a Repository?
        private IQueryable<Account> GetFilteredAccounts(AccountQuery filter, Paging paging)
        {
            IQueryable<Account> accounts = _db.Accounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
            {
                accounts = accounts.Where(a => a.FirstName.Contains(
                    filter.FirstName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.LastName))
            {
                accounts = accounts.Where(a => a.LastName.Contains(
                    filter.LastName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                accounts = accounts.Where(a => a.Email.Contains(
                    filter.Email, StringComparison.OrdinalIgnoreCase));
            }

            return accounts
                .OrderBy(a => a.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value);
        }

        public async Task<IServiceResponse<Account>> UpdateAccountAsync(
            int? id, AccountRequestDto request)
        {
            if (!IdValidator.IsValid(id) || !AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (_db.Accounts.FirstOrDefault(a => a.Id == id) is not Account account)
            {
                return new ServiceResponse<Account>(HttpStatusCode.NotFound);
            }

            try
            {
                return await UpdateAccountAndSaveChangesAsync(account, request);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account>();
            }
        }

        // TODO: Make this layer as a Service, and move the method into a Repository?
        private async Task<IServiceResponse<Account>> UpdateAccountAndSaveChangesAsync(
            Account account, AccountRequestDto dto)
        {
            AssignAccountData(account, dto);
            _db.Accounts.Attach(account);
            _db.Entry(account).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return new ServiceResponse<Account>(HttpStatusCode.OK, account);
        }

        private void AssignAccountData(Account account, AccountRequestDto newData)
        {
            account.FirstName = newData.FirstName!;
            account.LastName = newData.LastName!;
            account.Email = newData.Email!;
            // TODO: or if password == password then change the data?
        }

        public async Task<HttpStatusCode> RemoveAccountAsync(int? id, string? login)
        {
            if (!IdValidator.IsValid(id))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id) is not Account account)
            {
                return HttpStatusCode.Forbidden;
            }
            if (account.Email != login)
            {
                return HttpStatusCode.Forbidden;
            }

            if (await _db.Animals.FirstOrDefaultAsync(a => a.ChipperId == id) is Animal linkedAnimal)
            {
                return HttpStatusCode.BadRequest;
            }

            try
            {
                return await RemoveAccount(account);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        // TODO: repository layer?
        private async Task<HttpStatusCode> RemoveAccount(Account account)
        {
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }
    }
}
