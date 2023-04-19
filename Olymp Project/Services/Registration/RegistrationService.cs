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

        #region Register

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

            return await AddAccountToDatabaseAsync(request);
        }

        private async Task<bool> EmailAlreadyExists(string email)
        {
            return await _db.Accounts.AnyAsync(a => a.Email == email);
        }

        private async Task<IServiceResponse<Account>> AddAccountToDatabaseAsync(
            RegistrationRequestDto request)
        {
            try
            {
                Account newAccount = CreateAccount(request);
                await _db.Accounts.AddAsync(newAccount);
                await _db.SaveChangesAsync();

                return new ServiceResponse<Account>(HttpStatusCode.Created, newAccount);
            }
            catch (Exception)
            {
                return new ServiceResponse<Account>();
            }
        }

        private Account CreateAccount(RegistrationRequestDto data)
        {
            return new Account()
            {
                FirstName = data.FirstName!,
                LastName = data.LastName!,
                Email = data.Email!,
                Password = data.Password!
            };
        }

        public async Task<bool> ValidateAccountAsync(string email, string password)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
                if (account is not null && account.Password == password)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
