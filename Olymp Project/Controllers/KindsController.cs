using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Kinds;

namespace Olymp_Project.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [Route("animals/types")]
    [ApiController]
    public class KindsController : ControllerBase
    {
        private readonly IKindService _service;
        private readonly IMapper _mapper;

        public KindsController(IKindService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{kindId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KindResponseDto>> GetKind([FromRoute] long? kindId)
        {
            var response = await _service.GetAnimalKindByIdAsync(kindId!.Value);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> CreateKind([FromBody] KindRequestDto request)
        {
            var response = await _service.InsertAnimalKindAsync(request.Name!);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto, nameof(CreateKind));
        }

        [HttpPut("{kindId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = $"{Constants.Admin},{Constants.Chipper}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KindResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KindResponseDto>> UpdateKind(
            [FromRoute] long? kindId,
            [FromBody] KindRequestDto request)
        {
            var response = await _service.UpdateAnimalKindAsync(kindId!.Value, request.Name);

            var kindDto = _mapper.Map<KindResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, kindDto);
        }

        [HttpDelete("{kindId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, Roles = Constants.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKind([FromRoute] long? kindId)
        {
            var statusCode = await _service.RemoveAnimalKindByIdAsync(kindId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
