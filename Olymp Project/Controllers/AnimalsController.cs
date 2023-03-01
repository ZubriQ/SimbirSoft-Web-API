using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Animals;

namespace Olymp_Project.Controllers
{
    [Authorize]
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
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnimalResponseDto>> GetAnimal([FromRoute] long? animalId)
        {
            var response = await _service.GetAnimalAsync(animalId!.Value);

            var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(HttpStatusCode.OK, animalDto);
        }

        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimalResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AnimalResponseDto>>> GetAnimals(
            [FromQuery] AnimalQuery query,
            [FromQuery] Paging paging)
        {
            var response = await _service.GetAnimalsAsync(query, paging);

            var animalsDto = response.Data?.Select(a => _mapper.Map<AnimalResponseDto>(a));
            return ResponseHelper.GetActionResult(response.StatusCode, animalsDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AnimalResponseDto>> CreateAnimal([FromBody] PostAnimalDto request)
        {
            var response = await _service.InsertAnimalAsync(_mapper.Map<Animal>(request));

            var animalDto = _mapper.Map<AnimalResponseDto>(response.Data);
            // Should returns "visitedLocations": [], which seems OK.
            return ResponseHelper.GetActionResult(response.StatusCode, animalDto);
        }

        [HttpPut("{animalId:long}")]
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [Authorize(AuthenticationSchemes = ApiAuthenticationScheme.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAnimal([FromRoute] long? animalId)
        {
            var statusCode = await _service.RemoveAnimalAsync(animalId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
