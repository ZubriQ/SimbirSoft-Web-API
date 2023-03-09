using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Authentication;
using Olymp_Project.Services.Locations;

namespace Olymp_Project.Controllers
{
    [Authorize]
    [Route("locations")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _service;
        private readonly IMapper _mapper;

        public LocationsController(ILocationService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{locationId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationResponseDto>> GetLocation([FromRoute] long? locationId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.GetLocationByIdAsync(locationId!.Value);

            var locationDto = _mapper.Map<LocationResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, locationDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationResponseDto>> CreateLocation(
            [FromBody] LocationRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.InsertLocationAsync(request);

            var dto = _mapper.Map<LocationResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(CreateLocation));
        }

        [HttpPut("{locationId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationResponseDto>> UpdateLocation(
            [FromRoute] long? locationId,
            [FromBody] LocationRequestDto request)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.UpdateLocationAsync(locationId!.Value, request);

            var locationDto = _mapper.Map<LocationResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, locationDto);
        }

        [HttpDelete("{locationId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteLocation([FromRoute] long? locationId)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var statusCode = await _service.RemoveLocationAsync(locationId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
