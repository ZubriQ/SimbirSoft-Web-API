using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Registration;

namespace Olymp_Project.Controllers
{
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

        // TODO: 403 already authorized
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AccountResponseDto>> RegisterAccount(AccountRequestDto request)
        {
            var response = await _service.RegisterAccountAsync(_mapper.Map<Account>(request));

            var responseDto = _mapper.Map<AccountResponseDto>(response.Result);
            return ResponseHelper.GetActionResult(
                response.StatusCode, responseDto, nameof(RegisterAccount));
        }
    }
}
