using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Registration;
using System.Text;

namespace Olymp_Project.Controllers
{
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

            if (await IsAuthenticationValid() == HttpStatusCode.BadRequest)
            {
                return BadRequest();
            }

            var dto = _mapper.Map<AccountResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(RegisterAccount));
        }

        private async Task<HttpStatusCode> IsAuthenticationValid()
        {
            (var email, var password) = ExtractBasicAuthCredentials();
            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(email))
            {
                bool isAuthenticationValid = await _service.ValidateAccountAsync(email!, password!);
                if (!isAuthenticationValid)
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            return HttpStatusCode.OK;
        }

        private (string? email, string? password) ExtractBasicAuthCredentials()
        {
            // TODO: Optimize?
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedCredentials = authHeader.Substring(6);
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var separatorIndex = decodedCredentials.IndexOf(':');
                var email = decodedCredentials.Substring(0, separatorIndex);
                var password = decodedCredentials.Substring(separatorIndex + 1);

                return (email, password);
            }

            return (null!, null!);
        }
    }
}
