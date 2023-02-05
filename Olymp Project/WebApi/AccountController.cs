using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Database;
using Olymp_Project.DTOs.Account;

namespace Olymp_Project.WebApi
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static List<Account> _accountsStub = new List<Account>
        {
            new Account(0, "John", "Walter", "email1", "password1"),
            new Account(1, "Eugen", "Night", "email2", "password2"),
            new Account(2, "Josh", "White", "email3", "password3"),
            new Account(3, "Eugen", "Hammer", "email4", "password4"),
            new Account(4, "Vladimir", "Lennon", "email5", "password5"),
            new Account(5, "Eugen", "Hammer", "email6", "password6"),
            new Account(6, "John", "Lennon", "email7", "password7"),
            new Account(7, "Eugen", "Night", "email8", "password8"),
            new Account(8, "Eugen", "White", "email9", "password9"),
            new Account(9, "Eugen", "Night", "email10", "password10"),
            new Account(10, "Eugen", "White", "email11", "password11"),
        };

        [HttpGet("search")]
        public ActionResult<IEnumerable<Account>> Get([FromQuery] string? firstName,
                                                      [FromQuery] string? lastName,
                                                      [FromQuery] string? email,
                                                      [FromQuery] int? from = 0,
                                                      [FromQuery] int? size = 10)
        {
            if (CheckSearchParams(from, size))
            {
                return BadRequest();
            }

            #region Получение всех аккаунтов и фильтрация
            List<Account> accounts = _accountsStub.OrderBy(a => a.Id).ToList();

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                var filteredAccounts = accounts.Where(a => accounts.Any(b => a.FirstName.Contains(firstName)))
                                               .ToList();
                accounts = filteredAccounts;
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                var filteredAccounts = accounts.Where(a => accounts.Any(b => a.LastName.Contains(lastName)))
                                               .ToList();
                accounts = filteredAccounts;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var filteredAccounts = accounts.Where(a => accounts.Any(b => a.Email.Contains(email)))
                                               .ToList();
                accounts = filteredAccounts;
            }
            #endregion
            // TODO: return request 401: Неверные авторизационные данные.
            return Ok(accounts.Skip((int)from).Take((int)size));
        }

        private bool CheckSearchParams(int? from, int? size)
        {
            return !from.HasValue || from < 0 || !size.HasValue || size <= 0;
        }

        [HttpGet("{accountId:int}")]
        public ActionResult<Account> GetById(int? accountId)
        {
            if (!accountId.HasValue || accountId <= 0)
            {
                return BadRequest();
            }

            var account = _accountsStub.Where(a => a.Id == accountId)
                                       .OrderBy(a => a.Id)
                                       .FirstOrDefault();
            // TODO: 401: Неверные авторизационные данные.
            if (account == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(GetAccountDataForJson(account));
            }
        }

        private object GetAccountDataForJson(Account account)
        {
            return new
            {
                Id = account.Id,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email
            };
        }

        [HttpPut("{accountId:int}")]
        //public ActionResult<Account> Put(int accountId, string firstName, string lastName, string email, string password)
        public ActionResult<Account> Put(int accountId, [FromBody] UpdateAccountDto account)
        {
            //if (IsAccountIdInvalid(accountId) || AreAccountFieldsInvalid(account))
            //{
            //    return BadRequest();
            //}

            //var accountToUpdate = _accountsStub.Where(a => a.Id == accountId).FirstOrDefault();
            //if (accountToUpdate == null)
            //{
            //    return NotFound();
            //}
            //else if (_accountsStub.Any(a => a.Email == account.Email && a.Id != accountId))
            //{
            //    return Conflict();
            //}
            //else if (account.Password != accountToUpdate.Password) // TODO: Do I need this?
            //{
            //    return Unauthorized();
            //}

            //UpdateAccount(accountToUpdate, account);
            //return Ok(GetAccountDataForJson(accountToUpdate));
            // TODO: 401 unauthorized
            // TODO: check email аккаунта не валидный ? Regex?



            return Ok();
        }

        private bool IsAccountIdInvalid(int? id)
        {
            return !id.HasValue || id <= 0;
        }

        private bool AreAccountFieldsInvalid(Account account)
        {
            return string.IsNullOrWhiteSpace(account.FirstName)
                   || string.IsNullOrWhiteSpace(account.LastName)
                   || string.IsNullOrWhiteSpace(account.Email)
                   || string.IsNullOrWhiteSpace(account.Password);
        }

        private void UpdateAccount(Account account, Account newData)
        {
            account.FirstName = newData.FirstName;
            account.LastName = newData.LastName;
            account.Email = newData.Email;
        }
    }
}
