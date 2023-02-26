using Olymp_Project.Controllers.Validators;
using Olymp_Project.Models;
using Olymp_Project.Responses;
using System.Linq;
using System.Net;

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
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account is not null)
            {
                return new ServiceResponse<Account>(HttpStatusCode.OK, account);
            }
            else
            {
                return new ServiceResponse<Account>(HttpStatusCode.NotFound, null!);
            }
        }

        public async Task<IServiceResponse<ICollection<Account>>> GetAccountsAsync(
            AccountQuery query, 
            Paging paging)
        {
            if (!PagingValidator.IsValid(paging))
            {
                return new CollectionServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            try
            {
                // TODO: Test IQueryable vs IEnumerable.
                var accounts = GetFilteredAccounts(query, paging);
                return new CollectionServiceResponse<Account>(HttpStatusCode.OK, accounts);
            }
            catch (Exception)
            {
                return new CollectionServiceResponse<Account>();
            }
        }

        private IQueryable<Account> GetFilteredAccounts(AccountQuery query, Paging paging)
        {
            IQueryable<Account> accounts = _db.Accounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FirstName))
            {
                accounts = accounts.Where(a => a.FirstName.Contains(
                    query.FirstName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(query.LastName))
            {
                accounts = accounts.Where(a => a.LastName.Contains(
                    query.LastName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                accounts = accounts.Where(a => a.Email.Contains(
                    query.Email, StringComparison.OrdinalIgnoreCase));
            }

            return accounts
                .OrderBy(a => a.Id)
                .Skip(paging.Skip!.Value)
                .Take(paging.Take!.Value);
        }

        public async Task<IServiceResponse<Account>> UpdateAccountAsync(int? id, AccountRequestDto request)
        {
            if (!IdValidator.IsValid(id) || !AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            var account = _db.Accounts.FirstOrDefault(a => a.Id == id); // TODO: async?
            if (account is null)
            {
                return new ServiceResponse<Account>(HttpStatusCode.NotFound);
            }
            // TODO: optimize
            try
            {
                AssignNewData(account, request);

                _db.Accounts.Attach(account);
                _db.Entry(account).State = EntityState.Modified;
                await _db.SaveChangesAsync(); // TODO: async?
                return new ServiceResponse<Account>(HttpStatusCode.OK, account);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account>();
            }
        }

        private void AssignNewData(Account account, AccountRequestDto newData)
        {
            account.FirstName = newData.FirstName!;
            account.LastName = newData.LastName!;
            account.Email = newData.Email!;
            // TODO: change password too?
            // TODO: or if password == password then change the data?
        }

        public async Task<HttpStatusCode> RemoveAccountAsync(int? id)
        {
            if (!IdValidator.IsValid(id))
            {
                return HttpStatusCode.BadRequest;
            }

            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account is null)
            {
                return HttpStatusCode.Forbidden;
            }

            var linkedAnimal = await _db.Animals.FirstOrDefaultAsync(a => a.ChipperId == id);
            if (linkedAnimal is not null)
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

        private async Task<HttpStatusCode> RemoveAccount(Account account)
        {
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }
    }
}
