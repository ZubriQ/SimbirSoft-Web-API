using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Controllers.Validators;
using Olymp_Project.Dtos.Kind;
using Olymp_Project.Services.Kinds;
using System.Net;

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

        [HttpGet("{kindId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KindResponseDto>> GetKind(long? kindId)
        {
            if (!IdValidator.IsValid(kindId))
            {
                return BadRequest();
            }
            // TODO: 401
            var kind = await _service.GetAnimalKindAsync(kindId.Value);
            if (kind is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<KindResponseDto>(kind));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> AddKind(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }
            // TODO: 401: неавториз. акк; неверные авториз. данные.

            (HttpStatusCode status, var kind) = await _service.AddAnimalKindAsync(name);

            if (status is HttpStatusCode.Conflict)
            {
                return Conflict();
            }
            return Ok(_mapper.Map<KindResponseDto>(kind));
        }

        [HttpPut("{kindId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> UpdateKind(long? kindId, string? name)
        {
            if (!IdValidator.IsValid(kindId) || string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            // TODO: 401
            (HttpStatusCode status, Kind? kind) = 
                await _service.UpdateAnimalKindAsync(kindId.Value, name);

            switch(status)
            {
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.Conflict:
                    return Conflict();
            }
            return Ok(_mapper.Map<KindResponseDto>(kind));
        }

        [HttpDelete("{kindId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKind(long? kindId)
        {
            if (!IdValidator.IsValid(kindId))
            {
                return BadRequest();
            }

            var status = await _service.DeleteAnimalKindAsync(kindId.Value);
            switch(status)
            {
                case HttpStatusCode.BadRequest:
                    return BadRequest();
                case HttpStatusCode.NotFound:
                    return NotFound();
            }
            return Ok();
        }
    }
}
