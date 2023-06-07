using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirSoft_Web_API.Helpers;
using SimbirSoft_Web_API.Services.Registration;
using System.Text;

namespace SimbirSoft_Web_API.Controllers;

[Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
[Route("registration")]
[ApiController]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _service;
    private readonly IMapper _mapper;

    public RegistrationController(IRegistrationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponseDto>> RegisterAccount(
        [FromBody] RegistrationRequestDto request)
    {
        var response = await _service.RegisterAccountAsync(request);

        var dto = _mapper.Map<AccountResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(RegisterAccount));
    }
}
