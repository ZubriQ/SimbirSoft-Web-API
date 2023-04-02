using System.ComponentModel.DataAnnotations;

namespace Olymp_Project.Helpers.Validators
{
    public static class AccountValidator
    {
        public static bool IsValid(RegistrationRequestDto account)
        {
            return !string.IsNullOrWhiteSpace(account.FirstName)
                && !string.IsNullOrWhiteSpace(account.LastName)
                && !string.IsNullOrWhiteSpace(account.Password)
                && !string.IsNullOrWhiteSpace(account.Email)
                && new EmailAddressAttribute().IsValid(account.Email);
        }

        public static bool IsValid(AccountRequestDto account)
        {
            return !string.IsNullOrWhiteSpace(account.FirstName)
                && !string.IsNullOrWhiteSpace(account.LastName)
                && !string.IsNullOrWhiteSpace(account.Password)
                && !string.IsNullOrWhiteSpace(account.Email)
                && new EmailAddressAttribute().IsValid(account.Email)
                && (account.Role == "ADMIN" || account.Role == "CHIPPER" || account.Role == "USER");
        }
    }
}
