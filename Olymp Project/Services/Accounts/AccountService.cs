using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;
using System.Security.Claims;

namespace Olymp_Project.Services.Accounts
{
    public partial class AccountService : IAccountService
    {
        private readonly ChipizationDbContext _db;

        public AccountService(ChipizationDbContext db)
        {
            _db = db;
        }

        #region Get by id

        public async Task<IServiceResponse<Account>> GetAccountByIdAsync(int? accountId)
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

        public IServiceResponse<ICollection<Account>> GetAccounts(AccountQuery query, Paging paging)
        {
            if (!PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            try
            {
                var filteredAccounts = GetAccountsWithFilter(query);
                var pagedAccounts = PaginateAccounts(filteredAccounts, paging);
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

        private List<Account> PaginateAccounts(IQueryable<Account> accounts, Paging paging)
        {
            return accounts
                .OrderBy(a => a.Id)
                .Skip(paging.From!.Value)
                .Take(paging.Size!.Value)
                .ToList();
        }

        #endregion

        #region Insert

        public async Task<IServiceResponse<Account>> InsertAccountAsync(AccountRequestDto request)
        {
            if (!AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            // TODO: 403: У аккаунта нет роли "ADMIN"

            bool alreadyExists = await _db.Accounts.AnyAsync(a => a.Email ==  request.Email);
            if (alreadyExists)
            {
                return new ServiceResponse<Account>(HttpStatusCode.Conflict);
            }

            try
            {
                return await CreateAccountAnsSaveChangesAsync(request);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account>();
            }
        }

        private async Task<IServiceResponse<Account>> CreateAccountAnsSaveChangesAsync(AccountRequestDto request)
        {
            var newAccount = CreateAccount(request);
            _db.Accounts.Add(newAccount);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Account>(HttpStatusCode.Created, newAccount);
        }

        private Account CreateAccount(AccountRequestDto request)
        {
            var account = new Account()
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Email = request.Email!,
                Role = request.Role!,
                Password = request.Password!
            };
            return account;
        }

        //private async Task<IServiceResponse<Account>> AddAnimal(Animal animal)
        //{
        //    _db.Accounts.Add(animal);
        //    await _db.SaveChangesAsync();
        //    return new ServiceResponse<Animal>(HttpStatusCode.Created, animal);
        //}

        #endregion

        #region Update

        public async Task<IServiceResponse<Account>> UpdateAccountAsync(
            int? accountId, AccountRequestDto request, ClaimsIdentity? identity)
        {
            if (!IdValidator.IsValid(accountId) || !AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            var email = identity!.Name;
            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // TODO: Optimize
            if (await _db.Accounts.FindAsync(accountId) is not Account account)
            {
                if (role == "ADMIN")
                {
                    return new ServiceResponse<Account>(HttpStatusCode.NotFound);
                }
                else
                {
                    return new ServiceResponse<Account>(HttpStatusCode.Forbidden);
                }
            }

            if (role == "ADMIN")
            {
                return await UpdateAccountAndSaveChangesAsync(account, request);
            }
            else if (account.Email != email)
            {
                return new ServiceResponse<Account>(HttpStatusCode.Forbidden);
            }

            return await UpdateAccountAndSaveChangesAsync(account, request);

            //if (await _db.Accounts.FindAsync(accountId) is not Account account || account.Email != email)
            //{
            //    return new ServiceResponse<Account>(HttpStatusCode.Forbidden);
            //}

            //try
            //{
            //    return await UpdateAccountAndSaveChangesAsync(account, request);
            //}
            //catch (Exception)
            //{
            //    return new ServiceResponse<Account>();
            //}
        }

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
            account.Role = newData.Role!;
        }

        #endregion

        #region Remove

        public async Task<HttpStatusCode> RemoveAccountAsync(int? accountId, ClaimsIdentity? identity)
        {
            if (!IdValidator.IsValid(accountId) ||
                await _db.Animals.AnyAsync(a => a.ChipperId == accountId))
            {
                return HttpStatusCode.BadRequest;
            }

            var email = identity!.Name;
            var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            Account? account = await _db.Accounts.FindAsync(accountId);
            if (role == "ADMIN" && account is null)
            {
                return HttpStatusCode.NotFound;
            }
            else if ((role == "CHIPPER" || role == "USER") && account!.Email != email)
            {
                return HttpStatusCode.Forbidden;
            } 

            try
            {
                return await RemoveAccount(account!);
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private async Task<HttpStatusCode> RemoveAccount(Account account)
        {
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        #endregion
    }
}
