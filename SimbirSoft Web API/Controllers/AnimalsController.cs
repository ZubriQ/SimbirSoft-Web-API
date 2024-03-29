﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirSoft_Web_API.Helpers;
using SimbirSoft_Web_API.Services.Animals;

namespace SimbirSoft_Web_API.Controllers;

[Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
[Route("animals")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private readonly IAnimalService _service;
    private readonly IMapper _mapper;

    public AnimalsController(IAnimalService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{animalId:long}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnimalResponseDto>> GetAnimal([FromRoute] long? animalId)
    {
        var response = await _service.GetAnimalByIdAsync(animalId!.Value);

        var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, animalDto);
    }

    [HttpGet("search")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimalResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AnimalResponseDto>>> GetAnimals(
        [FromQuery] AnimalQuery query,
        [FromQuery] Paging paging)
    {
        var response = await _service.GetAnimalsBySearchParameters(query, paging);

        var animalsDto = response.Data?.Select(a => _mapper.Map<AnimalResponseDto>(a));
        return ResponseHelper.GetActionResult(response.StatusCode, animalsDto);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AnimalResponseDto>> CreateAnimal([FromBody] PostAnimalDto request)
    {
        var response = await _service.InsertAnimalAsync(_mapper.Map<Animal>(request));

        var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, animalDto, nameof(CreateAnimal));
    }

    [HttpPut("{animalId:long}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnimalResponseDto>> UpdateAnimal(
        [FromRoute] long? animalId,
        [FromBody] PutAnimalDto request)
    {
        var response = await _service.UpdateAnimalAsync(animalId!.Value, request);

        var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
        return ResponseHelper.GetActionResult(response.StatusCode, animalDto);
    }

    [HttpDelete("{animalId:long}")]
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = Constants.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnimal([FromRoute] long? animalId)
    {
        var statusCode = await _service.RemoveAnimalByIdAsync(animalId!.Value);
        return ResponseHelper.GetActionResult(statusCode);
    }
}
