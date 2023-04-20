using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Helpers.Geospatial;
using Olymp_Project.Services.Locations;

namespace Olymp_Project.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
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

        #region Default endpoints

        [HttpGet("{locationId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationResponseDto>> GetLocationById([FromRoute] long? locationId)
        {
            var response = await _service.GetLocationByIdAsync(locationId!.Value);

            var locationDto = _mapper.Map<LocationResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, locationDto);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(long))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<long>> GetLocationIdByCoordinates(
            [FromQuery] LocationRequestDto request)
        {
            var response = await _service.GetLocationIdByCoordinatesAsync(request);
            if (response.StatusCode is HttpStatusCode.OK)
            {
                return ResponseHelper.GetActionResult(HttpStatusCode.OK, response.Data!.Id);
            }

            return ResponseHelper.GetActionResult(response.StatusCode);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationResponseDto>> CreateLocation(
            [FromBody] LocationRequestDto request)
        {
            var response = await _service.InsertLocationAsync(request);

            var dto = _mapper.Map<LocationResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(CreateLocation));
        }

        [HttpPut("{locationId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationResponseDto>> UpdateLocation(
            [FromRoute] long? locationId,
            [FromBody] LocationRequestDto request)
        {
            var response = await _service.UpdateLocationAsync(locationId!.Value, request);

            var locationDto = _mapper.Map<LocationResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, locationDto);
        }

        [HttpDelete("{locationId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = Constants.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteLocation([FromRoute] long? locationId)
        {
            var statusCode = await _service.RemoveLocationAsync(locationId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }

        #endregion

        #region Geohash endpoints

        [HttpGet("geohash")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetGeohashByCoordinates(
            [FromQuery] LocationRequestDto request)
        {
            var response = await _service.GetLocationIdByCoordinatesAsync(request);
            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return ResponseHelper.GetActionResult(response.StatusCode);
            }

            string hash = GeohashHelper.Encode(response.Data!.Latitude, response.Data!.Longitude);
            return ResponseHelper.GetActionResult(response.StatusCode, hash);
        }

        [HttpGet("geohashv2")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetGeohashByCoordinatesV2(
            [FromQuery] LocationRequestDto request)
        {
            var response = await _service.GetLocationIdByCoordinatesAsync(request);
            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return ResponseHelper.GetActionResult(response.StatusCode);
            }

            string hash = GeohashHelper.EncodeV2(response.Data!.Latitude, response.Data!.Longitude);
            return ResponseHelper.GetActionResult(response.StatusCode, hash);
        }

        [HttpGet("geohashv3")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetGeohashByCoordinatesV3(
            [FromQuery] LocationRequestDto request)
        {
            var response = await _service.GetLocationIdByCoordinatesAsync(request);
            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return ResponseHelper.GetActionResult(response.StatusCode);
            }

            string hash = GeohashHelper.EncodeV3(response.Data!.Latitude, response.Data!.Longitude);
            return ResponseHelper.GetActionResult(response.StatusCode, hash);
        }

        #endregion
    }
}
