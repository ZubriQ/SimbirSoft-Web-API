using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Services.VisitedLocations;

namespace Olymp_Project.Controllers
{
    [Route("animals/{animalId:long}/locations")]
    [ApiController]
    public class VisitedLocationsController : ControllerBase
    {
        private readonly IVisitedLocationService _service;
        private readonly IMapper _mapper;

        public VisitedLocationsController(IVisitedLocationService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VisitedLocationResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VisitedLocationResponseDto>>> GetVisitedLocations(
            [FromRoute] long? animalId,
            [FromQuery] DateTimeRangeQuery query,
            [FromQuery] Paging paging)
        {
            if (!IdValidator.IsValid(animalId) || !PagingValidator.IsValid(paging))
            {
                return BadRequest();
            }
            // TODO: 401
             
            (HttpStatusCode code, var visitedLocations) = 
                await _service.GetVisitedLocationsAsync(animalId!.Value, query, paging);
            
            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return Ok(visitedLocations.Select(vl => _mapper.Map<VisitedLocationResponseDto>(vl)));
        }

        [HttpPost("{locationId:long}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VisitedLocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VisitedLocationResponseDto>> CreateVisitedLocation(
            [FromRoute] long? animalId,
            [FromRoute] long? locationId)
        {
            if (!IdValidator.IsValid(animalId, locationId))
            {
                return BadRequest();
            }
            // TODO: 401

            (HttpStatusCode code, var visitedLocation) =
                await _service.InsertVisitedLocationAsync(animalId!.Value, locationId!.Value);

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return CreatedAtAction(nameof(CreateVisitedLocation), 
                _mapper.Map<VisitedLocationResponseDto>(visitedLocation));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VisitedLocationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VisitedLocationResponseDto>> UpdateVisitedLocation(
            [FromRoute] long? animalId,
            [FromQuery] VisitedLocationRequestDto request)
        {
            if (!IdValidator.IsValid(animalId, request.VisitedLocationId, request.LocationId))
            {
                return BadRequest();
            }
            // TODO: 401

            (HttpStatusCode code, var updatedLocation) =
                await _service.UpdateVisitedLocationAsync(animalId!.Value, request);

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return Ok(_mapper.Map<VisitedLocationResponseDto>(updatedLocation));
        }

        [HttpDelete("{visitedLocationId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAnimalLocation(
            [FromRoute] long? animalId,
            [FromRoute] long? visitedLocationId)
        {
            if (!IdValidator.IsValid(animalId, visitedLocationId))
            {
                return BadRequest();
            }
            // TODO: 401

            var statusCode = await _service.RemoveVisitedLocationAsync(
                animalId!.Value, visitedLocationId!.Value);

            if (statusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
