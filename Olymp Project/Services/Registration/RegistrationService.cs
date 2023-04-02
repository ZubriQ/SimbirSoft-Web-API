using Olymp_Project.Helpers.Validators;
using Olymp_Project.Responses;

namespace Olymp_Project.Services.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ChipizationDbContext _db;

        public RegistrationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<IServiceResponse<Account>> RegisterAccountAsync(RegistrationRequestDto request)
        {
            if (!AccountValidator.IsValid(request))
            {
                return new ServiceResponse<Account>(HttpStatusCode.BadRequest);
            }

            if (await EmailAlreadyExists(request.Email!))
            {
                return new ServiceResponse<Account>(HttpStatusCode.Conflict);
            }

            try
            {
                return await InsertAccountAndSaveChangesAsync(request);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account>(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<bool> EmailAlreadyExists(string email)
        {
            return await _db.Accounts.AnyAsync(a => a.Email == email);
        }

        private async Task<IServiceResponse<Account>> InsertAccountAndSaveChangesAsync(
            RegistrationRequestDto request)
        {
            Account newAccount = CreateAccount(request);
            await _db.Accounts.AddAsync(newAccount);
            await _db.SaveChangesAsync();
            return new ServiceResponse<Account>(HttpStatusCode.Created, newAccount);
        }

        private Account CreateAccount(RegistrationRequestDto data)
        {
            Account newAccount = new Account()
            {
                FirstName = data.FirstName!,
                LastName = data.LastName!,
                Email = data.Email!,
                Password = data.Password!
            };
            return newAccount;
        }
    }
}
