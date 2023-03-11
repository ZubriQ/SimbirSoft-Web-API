namespace Olymp_Project.Authentication
{
    public class ApiAuthenticationService : IApiAuthenticationService
    {
        private readonly ChipizationDbContext _db;

        public ApiAuthenticationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<Account?> AuthenticateAccountAsync(string email, string password)
        {
            if (await _db.Accounts.FirstOrDefaultAsync(x => x.Email == email) is not Account user)
            {
                return Task.FromResult<Account>(null!).Result;
            }

            if (user.Password == password)
            {
                return user;
            }

            return Task.FromResult<Account>(null!).Result;
        }
    }
}
