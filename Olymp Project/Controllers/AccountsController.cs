using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Services.Accounts;

namespace Olymp_Project.Controllers
{
    //[Authorize]
    [Route("accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;
        private readonly IMapper _mapper;

        public AccountsController(IAccountService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{accountId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountResponseDto>> GetAccount(int? accountId) 
        {
            if (IdValidator.IsValid(accountId))
            {
                return BadRequest();
            }
            // TODO: 401: Неверные авторизационные данные.

            var account = await _service.GetAccountAsync(accountId!.Value);
            if (account is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountResponseDto>(account));
        }

        [HttpGet("search")]
        //[Authorize(AuthenticationSchemes = "Basic")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AccountResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts(
            [FromQuery] AccountQuery query,
            [FromQuery] Paging paging) // TODO: 401 unathorized, but allow anonymous?
        {
            if (!PagingValidator.IsValid(paging))
            {
                return BadRequest();
            }
            
            var accounts = await _service.GetAccountsAsync(query, paging);
            return Ok(accounts.Select(a => _mapper.Map<AccountResponseDto>(a)));
        }

        [HttpPut("{accountId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> UpdateAccount(
            int? accountId,
            AccountRequestDto account) // TODO: 401 unauthorized
        {
            if (!IdValidator.IsValid(accountId) || !AccountValidator.IsValid(_mapper.Map<Account>(account)))
            {
                return BadRequest();
            }

            var updatedAccount = await _service.UpdateAccountAsync(accountId!.Value, account);
            if (updatedAccount is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AccountResponseDto>(updatedAccount));
        }

        [HttpDelete("{accountId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAccount(int? accountId)
        {
            if (!IdValidator.IsValid(accountId))
            {
                return BadRequest();
            }
            // TODO: 401: unAuthorized.
            // TODO: 403: Удаление НЕ своего акка

            var status = await _service.RemoveAccountAsync(accountId!.Value);
            switch (status)
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
