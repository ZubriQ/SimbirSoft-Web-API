using SimbirSoft_Web_API.Responses;

namespace SimbirSoft_Web_API.Services.Registration
{
    public interface IRegistrationService
    {
        Task<IResponse<Account>> RegisterAccountAsync(RegistrationRequestDto request);
        Task<bool> ValidateAccountAsync(string email, string password);
    }
}
