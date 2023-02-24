using System.Net;

namespace Olymp_Project.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ChipizationDbContext _db;

        public RegistrationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<(HttpStatusCode, Account?)> RegisterAccountAsync(Account account)
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
    }
}
