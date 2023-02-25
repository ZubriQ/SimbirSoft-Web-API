using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Accounts;

namespace Olymp_Project.Controllers
{
    [Authorize]
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
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountResponseDto>> GetAccount(int? accountId) 
        {
            var response = await _service.GetAccountAsync(accountId!.Value);

            var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, accountDto);
        }

        // TODO: 401 unathorized, but allow anonymous?
        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AccountResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts(
            [FromQuery] AccountQuery query,
            [FromQuery] Paging paging)
        {
            var response = await _service.GetAccountsAsync(query, paging);

            var accountsDto = response.Data!.Select(a => _mapper.Map<AccountResponseDto>(a));
            return ResponseHelper.GetActionResult(response.StatusCode, accountsDto);
        }

        // TODO: 401 unauthorized   
        [HttpPut("{accountId:int}")]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> UpdateAccount(
            int? accountId,
            AccountRequestDto request)  
        {
            var response = await _service.UpdateAccountAsync(accountId!.Value, request);

            var accountDto = _mapper.Map<Account>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, accountDto);
        }

        // TODO: 401: unAuthorized.
        // TODO: 403: Удаление НЕ своего акка
        [HttpDelete("{accountId:int}")]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAccount(int? accountId)
        {
            var statusCode = await _service.RemoveAccountAsync(accountId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
