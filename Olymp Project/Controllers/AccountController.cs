using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Olymp_Project.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IChipizationService _chipService;
        private readonly IMapper _mapper;

        public AccountController(IChipizationService service, IMapper mapper)
        {
            _chipService = service;
            _mapper = mapper;
        }

        [HttpPost("~/registration")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> AddAccount(AccountRequestDto account)
        {
            if (AreAccountFieldsInvalid(account)
                || !new EmailAddressAttribute().IsValid(account.Email))
            {
                return BadRequest();
            }

            (HttpStatusCode status, Account? createdAccount) = 
                await _chipService.AddAccountAsync(_mapper.Map<Account>(account));

            switch (status)
            {
                case HttpStatusCode.Forbidden:
                    return Forbid();
                case HttpStatusCode.Conflict:
                    return Conflict();
            }
            // TODO: 403 already authorized
            // TODO: return real id, instead of id=0
            return Ok(_mapper.Map<AccountResponseDto>(createdAccount));
        }

        private bool AreAccountFieldsInvalid(AccountRequestDto account)
        {
            return string.IsNullOrWhiteSpace(account.FirstName) // TODO: A class that'd verify this, etc...
                   || string.IsNullOrWhiteSpace(account.LastName)
                   || string.IsNullOrWhiteSpace(account.Email)
                   || string.IsNullOrWhiteSpace(account.Password);
        }

        [HttpGet("{accountId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountResponseDto>> GetAccount(int? accountId)
        {
            if (!accountId.HasValue || accountId <= 0)
            {
                return BadRequest();
            }

            var account = await _chipService.GetAccountAsync((int)accountId);
            // TODO: 401: Неверные авторизационные данные.
            if (account == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountResponseDto>(account));
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AccountResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts(
            [FromQuery] AccountQuery query,
            [FromQuery] Paging paging)
        {
            if (CheckSearchParams(paging))
            {
                return BadRequest();
            }
            // TODO: 401 unathorized.
            var accounts = await _chipService.GetAccountsAsync(query, paging);
            return Ok(accounts.Select(a => _mapper.Map<AccountResponseDto>(a)));
        }

        private bool CheckSearchParams(Paging paging)
        {
            return !paging.From.HasValue || paging.From < 0 
                || !paging.Size.HasValue || paging.Size <= 0;
        }

        [HttpPut("{accountId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> UpdateAccount(
            int? accountId,
            AccountRequestDto account)
        {
            if (IsAccountIdInvalid(accountId) || AreAccountFieldsInvalid(account))
            {
                return BadRequest();
            }

            var updatedAccount = await _chipService.UpdateAccountAsync((int)accountId, account);
            if (updatedAccount == null)
            {
                return NotFound();
            }
            // TODO: 401 unauthorized
            // TODO: check email аккаунта не валидный ? Regex?
            return Ok(_mapper.Map<AccountResponseDto>(updatedAccount));
        }

        private bool IsAccountIdInvalid(int? id)
        {
            return !id.HasValue || id <= 0;
        }

        [HttpDelete("{accountId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAccount(int? accountId)
        {
            if (!accountId.HasValue || accountId <= 0)
            {
                return BadRequest();
            }
            // TODO: 401: unAuthorized.
            // TODO: 403: Удаление НЕ своего акка

            var result = await _chipService.DeleteAccountAsync((int)accountId);
            switch (result)
            {
                case HttpStatusCode.Forbidden:
                    return Forbid();
                case HttpStatusCode.BadRequest:
                    return BadRequest();
            }
            return Ok();
        }
    }
}
