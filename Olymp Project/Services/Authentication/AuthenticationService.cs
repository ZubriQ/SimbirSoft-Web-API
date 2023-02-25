namespace Olymp_Project.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ChipizationDbContext _db;

        public AuthenticationService(ChipizationDbContext db)
        {
            _db = db;
        }

        public async Task<Account?> AuthenticateAsync(string email, string password)
        {
            // TODO: user first or default?
            var user = await _db.Accounts.SingleOrDefaultAsync(x => x.Email == email);

            if (user is null)
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
