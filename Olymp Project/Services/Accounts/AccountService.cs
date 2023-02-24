using Olymp_Project.Models;
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

        public async Task<(HttpStatusCode, Account?)> InsertAccountAsync(Account account)
        {
            try
            {
                bool emailExists = _db.Accounts.Where(a => a.Email == account.Email).Any();
                if (emailExists)
                {
                    return (HttpStatusCode.Conflict, null);
                }
                // TODO: check if Email is valid. 400

                // TODO: add cancelation token?
                // (saves calculation time if the request is aborted)
                // https://stackoverflow.com/questions/71799601/context-addasync-savechangesasync-double-await

                await _db.Accounts.AddAsync(account);
                await _db.SaveChangesAsync();
                return (HttpStatusCode.OK, account);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<Account?> GetAccountAsync(int id)
        {
            return await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync(AccountQuery query, Paging paging)
        {
            try
            {
                return await GetFilteredAccounts(query, paging);
            }
            catch (Exception)
            {
                return Enumerable.Empty<Account>();
            }
        }

        private async Task<List<Account>> GetFilteredAccounts(AccountQuery query, Paging paging)
        {
            IQueryable<Account> accounts = _db.Accounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.FirstName))
            {
                accounts = accounts.Where(a => a.FirstName.Contains(query.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(query.LastName))
            {
                accounts = accounts.Where(a => a.LastName.Contains(query.LastName));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                accounts = accounts.Where(a => a.Email.Contains(query.Email));
            }

            return await accounts
                .OrderBy(a => a.Id)
                .Skip(paging.Skip!.Value)
                .Take(paging.Take!.Value)
                .ToListAsync();
        }

        public async Task<Account?> UpdateAccountAsync(int id, AccountRequestDto account)
        {
            try
            {
                var dbAccount = _db.Accounts.FirstOrDefault(a => a.Id == id); // TODO: async?
                if (dbAccount is null)
                {
                    return null;
                }

                return await UpdateAccount(dbAccount, account);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<Account> UpdateAccount(Account account, AccountRequestDto newData)
        {
            AssignNewData(account, newData);
            _db.Entry(account).State = EntityState.Modified;
            _db.SaveChanges(); // TODO: async?
            return account;
        }

        private void AssignNewData(Account account, AccountRequestDto newData)
        {
            account.FirstName = newData.FirstName!;
            account.LastName = newData.LastName!;
            account.Email = newData.Email!;
            // TODO: change password too?
            //account.Password = newData.Password;
        }

        public async Task<HttpStatusCode> RemoveAccountAsync(int id)
        {
            try
            {
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
