﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirSoft_Web_API.Helpers;
using SimbirSoft_Web_API.Services.VisitedLocations;

namespace SimbirSoft_Web_API.Controllers;

[Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
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
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VisitedLocationResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<VisitedLocationResponseDto>>> GetVisitedLocations(
        [FromRoute] long? animalId,
        [FromQuery] DateTimeRangeQuery query,
        [FromQuery] Paging paging)
    {
        var response = await _service.GetVisitedLocationsAsync(animalId!.Value, query, paging);

        var dto = response.Data?.Select(vl => _mapper.Map<VisitedLocationResponseDto>(vl));
        return ResponseHelper.GetActionResult(response.StatusCode, dto);
    }

    [HttpPost("{locationId:long}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VisitedLocationResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitedLocationResponseDto>> CreateVisitedLocation(
        [FromRoute] long? animalId,
        [FromRoute] long? locationId)
    {
        var response = await _service.InsertVisitedLocationAsync(animalId!.Value, locationId!.Value);

        var dto = _mapper.Map<VisitedLocationResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(CreateVisitedLocation));
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VisitedLocationResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VisitedLocationResponseDto>> UpdateVisitedLocation(
        [FromRoute] long? animalId,
        [FromBody] VisitedLocationRequestDto request)
    {
        var response = await _service.UpdateVisitedLocationAsync(animalId!.Value, request);

        var dto = _mapper.Map<VisitedLocationResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, dto);
    }

    [HttpDelete("{visitedLocationId:long}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = Constants.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnimalLocation(
        [FromRoute] long? animalId,
        [FromRoute] long? visitedLocationId)
    {
        var statusCode = await _service.RemoveVisitedLocationAsync(
            animalId!.Value, visitedLocationId!.Value);
        return ResponseHelper.GetActionResult(statusCode);
    }
}
