﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Helpers;
using Olymp_Project.Services.AnimalsKinds;

namespace Olymp_Project.Controllers
{
    [Authorize]
    [Route("animals/{animalId:long}/types/")]
    [ApiController]
    public class AnimalsKindsController : ControllerBase
    {
        private readonly IAnimalKindService _service;
        private readonly IMapper _mapper;

        public AnimalsKindsController(IAnimalKindService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("{kindId}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AnimalResponseDto>> CreateAnimalKind(
            [FromRoute] long? animalId,
            [FromRoute] long? kindId)
        {
            var response = await _service.InsertKindToAnimalAsync(animalId!.Value, kindId!.Value);

            var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(
                response.StatusCode, animalDto, nameof(CreateAnimalKind));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AnimalResponseDto>> UpdateAnimalKind(
            [FromRoute] long? animalId,
            [FromBody] PutAnimalKindDto request)
        {
            var response = await _service.UpdateAnimalKindAsync(animalId!.Value, request);

            var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, animalDto);
        }

        [HttpDelete("{kindId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnimalResponseDto>> DeleteAnimalKind(
            [FromRoute] long? animalId,
            [FromRoute] long? kindId)
        {
            var response = await _service.RemoveAnimalKindAsync(animalId!.Value, kindId!.Value);

            var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(HttpStatusCode.OK, animalDto);
        }
    }
}
