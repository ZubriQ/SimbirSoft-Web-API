using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Accounts;
using Olymp_Project.Services.Authentication;
using System.Security.Claims;

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
        [Authorize(AuthenticationSchemes = Constants.BasicAuthenticationScheme)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountResponseDto>> GetAccount([FromRoute] int? accountId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.GetAccountByIdAsync(accountId!.Value);

            var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, accountDto);
        }

        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthenticationScheme)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AccountResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts(
            [FromQuery] AccountQuery query,
            [FromQuery] Paging paging)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            //var role = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            //if (role is not null && (role != "ADMIN"))
            //{
            //    return Forbid();
            //}

            var response = _service.GetAccounts(query, paging);

            var accountsDto = response.Data!.Select(a => _mapper.Map<AccountResponseDto>(a));
            return ResponseHelper.GetActionResult(response.StatusCode, accountsDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> CreateAccount([FromBody] AccountRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.InsertAccountAsync(request);

            var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, accountDto, nameof(CreateAccount));
        }

        [HttpPut("{accountId:int}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> UpdateAccount(
            [FromRoute] int? accountId,
            [FromBody] AccountRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            //var role = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            //if (role is not null && (role != ))
            //{

            //}

            var response = await _service.UpdateAccountAsync(
                accountId!.Value, request, User.Identity as ClaimsIdentity);

            var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, accountDto);
        }

        [HttpDelete("{accountId:int}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAccount([FromRoute] int? accountId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var statusCode = await _service.RemoveAccountAsync(
                accountId!.Value, User.Identity as ClaimsIdentity);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
