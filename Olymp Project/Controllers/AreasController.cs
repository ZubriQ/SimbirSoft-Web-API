using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Areas;
using Olymp_Project.Services.Authentication;

namespace Olymp_Project.Controllers
{
    [Authorize]
    [Route("areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService _service;
        private readonly IMapper _mapper;

        public AreasController(IAreaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{areaId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AreaResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AreaResponseDto>> GetArea([FromRoute] long? areaId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.GetAreaByIdAsync(areaId!.Value);

            var areaDto = _mapper.Map<AreaResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, areaDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AreaResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AreaResponseDto>> CreateArea([FromBody] AreaRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.InsertAreaAsync(request);

            var areaDto = _mapper.Map<AreaResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, areaDto, nameof(CreateArea));
        }

        [HttpPut("{areaId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AreaResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AreaResponseDto>> UpdateArea(
            [FromRoute] long? areaId,
            [FromBody] AreaRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.UpdateAreaByIdAsync(areaId!.Value, request);

            var kindDto = _mapper.Map<AreaResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto);
        }


        [HttpDelete("{areaId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKind([FromRoute] long? areaId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var statusCode = await _service.RemoveAreaByIdAsync(areaId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
