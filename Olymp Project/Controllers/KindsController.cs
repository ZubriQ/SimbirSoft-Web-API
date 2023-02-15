using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Dtos.Kind;
using Olymp_Project.Services.Kinds;

namespace Olymp_Project.Controllers
{
    [Route("animals/types")]
    [ApiController]
    public class KindsController : ControllerBase
    {
        private readonly IKindService _service;
        private readonly IMapper _mapper;

        public KindsController(IMapper mapper, IKindService kindService)
        {
            _mapper = mapper;
            _service = kindService;
        }

        [HttpGet("{typeId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KindResponseDto>> GetKindById(long? typeId)
        {
            if (!IdValidator.IsValid(typeId))
            {
                return BadRequest();
            }

            var kind = await _service.GetAnimalKindAsync(typeId.Value);
            if (kind is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<KindResponseDto>(kind));
        }
    }
}
