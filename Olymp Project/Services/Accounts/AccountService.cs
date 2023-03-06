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

        public async Task<IServiceResponse<Account>> GetAccountAsync(int? accountId)
        {
            if (!IdValidator.IsValid(accountId))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (await _db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId) is not Account account)
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
                var accounts = await GetFilteredAccounts(query, paging);
                return new CollectionServiceResponse<Account>(HttpStatusCode.OK, accounts);
            }
            catch (Exception)
            {
                return new CollectionServiceResponse<Account>();
            }
        }

        // TODO: Make this layer as a Service, and move the method into a Repository?
        private async Task<IQueryable<Account>> GetFilteredAccounts(AccountQuery filter, Paging paging)
        {
            IQueryable<Account> accounts = _db.Accounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FirstName))
            {
                string firstNameLower = filter.FirstName.ToLower();
                accounts = accounts.Where(a => a.FirstName.ToLower().Contains(firstNameLower));
            }

            if (!string.IsNullOrWhiteSpace(filter.LastName))
            {
                string lastNameLower = filter.LastName.ToLower();
                accounts = accounts.Where(a => a.LastName.ToLower().Contains(lastNameLower));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                string emailLower = filter.Email.ToLower();
                accounts = accounts.Where(a => a.Email.ToLower().Contains(emailLower));
            }

            return accounts
                .OrderBy(a => a.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value);
        }

        public async Task<IServiceResponse<Account>> UpdateAccountAsync(
            int? accountId, AccountRequestDto request, string? login)
        {
            if (!IdValidator.IsValid(accountId) || !AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (await _db.Accounts.FindAsync(accountId) is not Account account)
            {
                return new ServiceResponse<Account>(HttpStatusCode.Forbidden);
            }
            if (account.Email != login)
            {
                return new ServiceResponse<Account>(HttpStatusCode.Forbidden);
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
            account.Password = newData.Password!;
        }

        public async Task<HttpStatusCode> RemoveAccountAsync(int? accountId, string? email)
        {
            if (!IdValidator.IsValid(accountId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await _db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId) is not Account account)
            {
                return HttpStatusCode.Forbidden;
            }
            if (account.Email != email)
            {
                return HttpStatusCode.Forbidden;
            }

            if (await _db.Animals.FirstOrDefaultAsync(a => a.ChipperId == accountId) is Animal linkedAnimal)
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
