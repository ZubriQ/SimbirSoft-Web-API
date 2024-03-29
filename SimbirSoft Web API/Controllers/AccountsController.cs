﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirSoft_Web_API.Helpers;
using SimbirSoft_Web_API.Services.Accounts;
using System.Security.Claims;

namespace SimbirSoft_Web_API.Controllers;

[Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
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
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountResponseDto>> GetAccount([FromRoute] int? accountId)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if ((userRole == Constants.User || userRole == Constants.Chipper) && accountId.ToString() != userId)
        {
            return Forbid();
        }

        var response = await _service.GetAccountByIdAsync(accountId!.Value);

        if (response.StatusCode == HttpStatusCode.NotFound && userRole == Constants.Admin)
        {
            return NotFound();
        }

        var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, accountDto);
    }

    [HttpGet("search")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = Constants.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AccountResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAccounts(
        [FromQuery] AccountQuery query,
        [FromQuery] Paging paging)
    {
        var response = await _service.GetAccountsBySearchParametersAsync(query, paging);

        var accountsDto = response.Data!.Select(a => _mapper.Map<AccountResponseDto>(a));
        return ResponseHelper.GetActionResult(response.StatusCode, accountsDto);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = Constants.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponseDto>> CreateAccount([FromBody] AccountRequestDto request)
    {
        var response = await _service.InsertAccountAsync(request);

        var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, accountDto, nameof(CreateAccount));
    }

    [HttpPut("{accountId:int}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponseDto>> UpdateAccount(
        [FromRoute] int? accountId,
        [FromBody] AccountRequestDto request)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if ((userRole == Constants.User || userRole == Constants.Chipper) && accountId.ToString() != userId)
        {
            return Forbid();
        }

        var response = await _service.UpdateAccountAsync(accountId, request);
        if (response.StatusCode == HttpStatusCode.NotFound && userRole == Constants.Admin)
        {
            return NotFound();
        }

        var accountDto = _mapper.Map<AccountResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, accountDto);
    }

    [HttpDelete("{accountId:int}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount([FromRoute] int? accountId)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if ((userRole == Constants.User || userRole == Constants.Chipper) && accountId.ToString() != userId)
        {
            return Forbid();
        }

        var statusCode = await _service.RemoveAccountAsync(accountId);
        if (statusCode == HttpStatusCode.NotFound && userRole == Constants.Admin)
        {
            return NotFound();
        }

        return ResponseHelper.GetActionResult(statusCode);
    }
}
