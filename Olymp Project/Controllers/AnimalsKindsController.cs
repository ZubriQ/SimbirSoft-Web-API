using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Services.AnimalsKinds;

namespace Olymp_Project.Controllers
{
    [Route("animals/{animalId:long}/types/")]
    [ApiController]
    public class AnimalsKindsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAnimalKindService _service;

        public AnimalsKindsController(IAnimalKindService service, IMapper mapper)
        {
            _mapper = mapper;
            _service = service;
        }

        [HttpPost("{kindId}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AnimalResponseDto>> AddAnimalKind(
            [FromRoute] long? animalId,
            [FromRoute] long? kindId)
        {
            if (!IdValidator.IsValid(animalId) || !IdValidator.IsValid(kindId))
            {
                return BadRequest();
            }

            (HttpStatusCode code, Animal? animal) =
                await _service.InsertKindToAnimalAsync(animalId!.Value, kindId!.Value);

            switch (code)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.Conflict:
                    return Conflict();
            }
            return CreatedAtAction(nameof(AddAnimalKind), _mapper.Map<AnimalResponseDto>(animal));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AnimalResponseDto>> UpdateAnimalKind(
            [FromRoute] long? animalId,
            PutAnimalKindDto request)
        {
            if (!IdValidator.IsValid(animalId, request.OldKindId, request.NewKindId))
            {
                return BadRequest();
            }
            // TODO: 401

            (HttpStatusCode code, Animal? updatedAnimal) =
                await _service.UpdateAnimalKindAsync(animalId!.Value, request);

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.Conflict:
                    return Conflict();
            }
            return Ok(_mapper.Map<AnimalResponseDto>(updatedAnimal));
        }

        [HttpDelete("{kindId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnimalResponseDto>> DeleteAnimalKind(
            [FromRoute] long? animalId,
            [FromRoute] long? kindId)
        {
            if (!IdValidator.IsValid(animalId) || !IdValidator.IsValid(kindId))
            {
                return BadRequest();
            }
            // TODO: 401

            (HttpStatusCode code, Animal? deletedAnimal) =
                await _service.RemoveAnimalKindAsync(animalId!.Value, kindId!.Value);

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return Ok(_mapper.Map<AnimalResponseDto>(deletedAnimal));
        }
    }
}
