using System.Net;

namespace Olymp_Project.Services
{
    public partial class ChipizationService : IChipizationService
    {
        private readonly AnimalChipizationContext _db;

        public ChipizationService(AnimalChipizationContext db)
        {
            _db = db;
        }

        #region Accounts

        public async Task<(HttpStatusCode, Account?)> AddAccountAsync(Account account)
        {
            try
            {
                bool emailExists = _db.Accounts.Where(a => a.Email == account.Email).Any();
                if (emailExists)
                {
                    return (HttpStatusCode.Conflict, null);
                }
                // TODO: check if Email is valid. 400

                await _db.Accounts.AddAsync(account);
                return (HttpStatusCode.OK, account);
            }
            catch (Exception)
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public async Task<Account?> GetAccountAsync(int id)
        {
            try
            {
                return await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
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
            List<Account> accounts = await _db.Accounts.ToListAsync();

            if (!string.IsNullOrWhiteSpace(query.FirstName) && accounts.Count > 0)
            {
                var filteredAccounts = accounts.Where(a => accounts.Any(b => a.FirstName.Contains(query.FirstName)))
                                               .ToList();
                accounts = filteredAccounts;
            }

            if (!string.IsNullOrWhiteSpace(query.LastName) && accounts.Count > 0)
            {
                var filteredAccounts = accounts.Where(a => accounts.Any(b => a.LastName.Contains(query.LastName)))
                                               .ToList();
                accounts = filteredAccounts;
            }

            if (!string.IsNullOrWhiteSpace(query.Email) && accounts.Count > 0)
            {
                var filteredAccounts = accounts.Where(a => accounts.Any(b => a.Email.Contains(query.Email)))
                                               .ToList();
                accounts = filteredAccounts;
            }

            return accounts.Skip((int)paging.From).Take((int)paging.Size).OrderBy(a => a.Id).ToList();
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

                UpdateAccount(dbAccount, account);
                _db.Entry(dbAccount).State = EntityState.Modified;
                _db.SaveChanges(); // TODO: async?

                return dbAccount;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void UpdateAccount(Account account, AccountRequestDto newData)
        {
            account.FirstName = newData.FirstName;
            account.LastName = newData.LastName;
            account.Email = newData.Email;
            // TODO: change password too?
            //account.Password = newData.Password;
        }

        public async Task<HttpStatusCode> DeleteAccountAsync(int id)
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

        #region Animals

        public async Task<Animal?> GetAnimalAsync(long id)
        {
            try
            {
                return await _db.Animals
                    .AsNoTracking()
                    .Include(a => a.VisitedLocations)
                    .Include(a => a.Types)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
