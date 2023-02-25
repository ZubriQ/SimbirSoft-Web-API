using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
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

        [HttpGet("{pointId:long}")]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationResponseDto>> GetLocation(long? pointId) 
        {
            if (!IdValidator.IsValid(pointId))
            {
                return BadRequest();
            }

            // TODO: 401 unauthorized
            var location = await _service.GetLocationAsync(pointId.Value);
            if (location is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<LocationResponseDto>(location));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationResponseDto>> CreateLocation(LocationRequestDto location)
        {
            if (!LocationValidator.IsValid(location))
            {
                return BadRequest();
            }
            // TODO: 401 unauthorized;
            // invalid credentials.

            (HttpStatusCode status, Location? addedLocation) = 
                await _service.AddLocationAsync(_mapper.Map<Location>(location));

            if (status == HttpStatusCode.Conflict)
            {
                return Conflict();
            }
            var mappedLocation = _mapper.Map<LocationResponseDto>(addedLocation);
            return CreatedAtAction(nameof(CreateLocation), mappedLocation);
        }

        [HttpPut("{pointId:long}")]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationResponseDto>> UpdateLocation(
            long? pointId,
            LocationRequestDto location)
        {
            if (!IdValidator.IsValid(pointId) || !LocationValidator.IsValid(location))
            {
                return BadRequest();
            }
            // TODO: 401 unauthorized;
            // invalid credentials.

            (HttpStatusCode status, Location? updatedLocation) =
                await _service.UpdateLocationAsync(pointId.Value, _mapper.Map<Location>(location));

            switch (status)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.Conflict:
                    return Conflict();
            }
            return Ok(_mapper.Map<LocationResponseDto>(updatedLocation));
        }

        [HttpDelete("{pointId:long}")]
        [Authorize(AuthenticationSchemes = AuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteLocation(long? pointId)
        {
            if (!IdValidator.IsValid(pointId))
            {
                return BadRequest();
            }
            // TODO: 401 unauthorized;
            // invalid credentials.

            var status = await _service.RemoveLocationAsync(pointId.Value);
            switch (status)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.BadRequest:
                    return BadRequest();
            }
            return Ok();
        }
    }
}
