using Olymp_Project.Controllers.Validators;
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

        public async Task<ServiceResponse<Account?>> RegisterAccountAsync(Account account)
        {
            if (!AccountValidator.IsValid(account))
            {
                return new ServiceResponse<Account?>(HttpStatusCode.BadRequest);
            }

            bool emailExists = _db.Accounts.Where(a => a.Email == account.Email).Any();
            if (emailExists)
            {
                return new ServiceResponse<Account?>(HttpStatusCode.Conflict);
            }

            try
            {
                await _db.Accounts.AddAsync(account);
                await _db.SaveChangesAsync();
                return new ServiceResponse<Account?>(HttpStatusCode.Created, account);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account?>(HttpStatusCode.InternalServerError);
            }
        }
    }
}
