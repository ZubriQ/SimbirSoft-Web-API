using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Services.Animals;
using System.Net;

namespace Olymp_Project.Controllers
{
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

        [HttpGet("{animalId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnimalResponseDto>> GetAnimal(long? animalId)
        {
            if (!IdValidator.IsValid(animalId))
            {
                return BadRequest();
            }
            // TODO: 401: Неверные авторизационные данные.

            var animal = await _service.GetAnimalAsync(animalId.Value);
            if (animal is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AnimalResponseDto>(animal));
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimalResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AnimalResponseDto>>> GetAnimals(
            [FromQuery] AnimalQuery query,
            [FromQuery] Paging paging)
        {
            // TODO: CHECK THE DATETIMES.
            if (!PagingValidator.IsValid(paging) || !AnimalValidator.IsQueryValid(query))
            {
                return BadRequest();
            }

            var animals = await _service.GetAnimalsAsync(query, paging);
            return Ok(animals.Select(a => _mapper.Map<AnimalResponseDto>(a)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AnimalResponseDto>> CreateAnimal(PostAnimalDto request)
        {
            if (!AnimalValidator.IsRequestValid(request))
            {
                return BadRequest();
            }
            // TODO: 401

            (HttpStatusCode status, Animal? createdAnimal) = 
                await _service.InsertAnimalAsync(_mapper.Map<Animal>(request));

            switch (status)
            {
                case HttpStatusCode.Conflict:
                    return Conflict();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            // Returns "visitedLocations": [], which seems OK.
            return CreatedAtAction(nameof(CreateAnimal), 
                _mapper.Map<AnimalResponseDto>(createdAnimal));
        }

        [HttpPut("{animalId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnimalResponseDto>> UpdateAnimal(
            long? animalId, 
            PutAnimalDto request)
        {
            if (!IdValidator.IsValid(animalId) || !AnimalValidator.IsRequestValid(request))
            {
                return BadRequest();
            }

            (HttpStatusCode code, Animal? updatedAnimal) =
                await _service.UpdateAnimalAsync(animalId!.Value, request);

            switch (code)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return Ok(_mapper.Map<AnimalResponseDto>(updatedAnimal));
        }
    }
}
