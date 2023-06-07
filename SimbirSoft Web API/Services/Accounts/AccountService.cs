using SimbirSoft_Web_API.Helpers.Validators;
using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Accounts
{
    public partial class AccountService : IAccountService
    {
        private readonly ChipizationDbContext _db;

        public AccountService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get by id

        public async Task<IResponse<Account>> GetAccountByIdAsync(int? accountId)
        {
            if (!IdValidator.IsValid(accountId))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (await _db.Accounts.FindAsync(accountId) is not Account account)
            {
                return new ServiceResponse<Account>(HttpStatusCode.NotFound);
            }

            return new ServiceResponse<Account>(HttpStatusCode.OK, account);
        }

        #endregion

        #region Get by search parameters

        public async Task<IResponse<ICollection<Account>>> GetAccountsBySearchParametersAsync(AccountQuery query, Paging paging)
        {
            if (!PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            return await GetAccountsBySearchParameters(query, paging);
        }

        private async Task<IResponse<ICollection<Account>>> GetAccountsBySearchParameters(
            AccountQuery query, Paging paging)
        {
            try
            {
                var filteredAccounts = GetAccountsWithFilter(query);
                var pagedAccounts = await PaginateAccounts(filteredAccounts, paging);
                return new CollectionServiceResponse<Account>(HttpStatusCode.OK, pagedAccounts);
            }
            catch (Exception)
            {
                return new CollectionServiceResponse<Account>();
            }
        }

        private IQueryable<Account> GetAccountsWithFilter(AccountQuery filter)
        {
            string loweredFirstName = string.IsNullOrWhiteSpace(filter.FirstName) ? null! : filter.FirstName.ToLower();
            string loweredLastName = string.IsNullOrWhiteSpace(filter.LastName) ? null! : filter.LastName.ToLower();
            string loweredEmail = string.IsNullOrWhiteSpace(filter.Email) ? null! : filter.Email.ToLower();

            return GetFilteredAccounts(loweredFirstName, loweredLastName, loweredEmail);
        }

        private IQueryable<Account> GetFilteredAccounts(string? firstName, string? lastName, string? email)
        {
            return _db.Accounts
                .AsQueryable()
                .Where(a =>
                    (firstName == null || a.FirstName.ToLower().Contains(firstName)) &&
                    (lastName == null || a.LastName.ToLower().Contains(lastName)) &&
                    (email == null || a.Email.ToLower().Contains(email)));
        }

        private async Task<List<Account>> PaginateAccounts(IQueryable<Account> accounts, Paging paging)
        {
            return await accounts
                .OrderBy(a => a.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value)
                .ToListAsync();
        }

        #endregion

        #region Insert

        public async Task<IResponse<Account>> InsertAccountAsync(AccountRequestDto request)
        {
            if (!AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            bool alreadyExists = await _db.Accounts.AnyAsync(a => a.Email ==  request.Email);
            if (alreadyExists)
            {
                return new ServiceResponse<Account>(HttpStatusCode.Conflict);
            }

            return await CreateAccountAnsSaveChangesAsync(request);
        }

        private async Task<IResponse<Account>> CreateAccountAnsSaveChangesAsync(AccountRequestDto request)
        {
            try
            {
                var newAccount = CreateAccount(request);
                _db.Accounts.Add(newAccount);
                await _db.SaveChangesAsync();

                return new ServiceResponse<Account>(HttpStatusCode.Created, newAccount);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account>();
            }
        }

        private Account CreateAccount(AccountRequestDto request)
        {
            return new Account()
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Email = request.Email!,
                Role = request.Role!,
                Password = request.Password!
            };
        }

        #endregion

        #region Update

        public async Task<IResponse<Account>> UpdateAccountAsync(
            int? accountId, AccountRequestDto request)
        {
            if (!IdValidator.IsValid(accountId) || !AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (await _db.Accounts.FindAsync(accountId) is not Account account)
            {
                return new ServiceResponse<Account>(HttpStatusCode.NotFound);
            }

            return await UpdateAccountAndSaveChangesAsync(account, request);
        }

        private async Task<IResponse<Account>> UpdateAccountAndSaveChangesAsync(
            Account account, AccountRequestDto request)
        {
            account.FirstName = request.FirstName!;
            account.LastName = request.LastName!;
            account.Email = request.Email!;
            account.Password = request.Password!;
            account.Role = request.Role!;

            _db.Accounts.Update(account);
            await _db.SaveChangesAsync();

            return new ServiceResponse<Account>(HttpStatusCode.OK, account);
        }

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveAccountAsync(int? accountId)
        {
            if (!IdValidator.IsValid(accountId) ||
                await _db.Animals.AnyAsync(a => a.ChipperId == accountId))
            {
                return HttpStatusCode.BadRequest;
            }

            if (await _db.Accounts.FindAsync(accountId) is not Account account)
            {
                return HttpStatusCode.NotFound;
            }

            return await RemoveAccountAndSaveChangesAsync(account);
        }

        private async Task<HttpStatusCode> RemoveAccountAndSaveChangesAsync(Account account)
        {
            try
            {
                _db.Accounts.Remove(account);
                await _db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        #endregion
    }
}
