using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Olymp_Project.Controllers
{
    [Route("animals")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IChipizationService _chipService;
        private readonly IMapper _mapper;

        public AnimalController(IChipizationService service, IMapper mapper)
        {
            _chipService = service;
            _mapper = mapper;
        }

        [HttpGet("{animalId:int}")]
        public async Task<ActionResult<GetAnimalDto>> GetAnimal(long? animalId)
        {
            if (!animalId.HasValue || animalId <= 0)
            {
                return BadRequest();
            }

            //var animalDto = await _chipService.GetAnimalAsync(animalId);
            var animal = await _chipService.GetAnimalAsync((long)animalId);
            
            // TODO: 401: Неверные авторизационные данные.
            if (animal == null)
            {
                return NotFound();
            }
            else
            {
                var animalDto = _mapper.Map<GetAnimalDto>(animal);
                return Ok(animalDto);
            }
        }

        //private async Task<GetAnimalDto?> FindAnimalDataById(long? animalId)
        //{
        //    return await null;
        //}
    }
}
