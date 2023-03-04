using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Authentication;
using Olymp_Project.Services.Kinds;

namespace Olymp_Project.Controllers
{
    [Authorize]
    [Route("animals/types")]
    [ApiController]
    public class KindsController : ControllerBase
    {
        private readonly IKindService _service;
        private readonly IMapper _mapper;

        public KindsController(IKindService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KindResponseDto>> GetKind([FromRoute] long? kindId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.GetAnimalKindAsync(kindId!.Value);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> CreateKind([FromBody] KindRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.InsertAnimalKindAsync(request.Type!);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto, nameof(CreateKind));
        }

        [HttpPut("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> UpdateKind(
            [FromRoute] long? kindId,
            [FromBody] KindRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.UpdateAnimalKindAsync(kindId!.Value, request.Type);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto);
        }

        [HttpDelete("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKind(long? kindId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var statusCode = await _service.RemoveAnimalKindAsync(kindId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
