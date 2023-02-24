using System.ComponentModel.DataAnnotations;

namespace Olymp_Project.Controllers.Validators
{
    public static class AccountValidator
    {
        public static bool IsValid(AccountRequestDto account)
        {
            return !string.IsNullOrWhiteSpace(account.FirstName)
                && !string.IsNullOrWhiteSpace(account.LastName)
                && !string.IsNullOrWhiteSpace(account.Password)
                && !string.IsNullOrWhiteSpace(account.Email)
                && new EmailAddressAttribute().IsValid(account.Email);
        }
    }
}
