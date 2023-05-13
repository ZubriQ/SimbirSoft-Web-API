using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Dtos.Path;
using Olymp_Project.Helpers;
using Olymp_Project.Services.Paths;
using System.Security.Claims;

namespace Olymp_Project.Controllers
{
    /// <summary>
    /// A hypothetical controller to implement in the 3rd stage of the contest.
    /// </summary>
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
    [Route("hypothetical-stage3-endpoint/paths")]
    [ApiController]
    public class PathsController : ControllerBase
    {
        private readonly IPathService _service;
        private readonly IMapper _mapper;

        public PathsController(IPathService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PathResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PathResponseDto>> CreatePath([FromBody] PathRequestDto request)
        {
            var response = await _service.InsertPathAsync(request);

            var dto = _mapper.Map<PathResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(CreatePath));
        }

        [HttpGet("{pathId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PathResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PathResponseDto>> GetPath([FromRoute] long? pathId)
        {
            var response = await _service.GetPathByIdAsync(pathId!.Value);

            var dto = _mapper.Map<PathResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto);
        }

        [HttpGet("all")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<PathResponseDto>))]
        public async Task<ActionResult<ICollection<PathResponseDto>>> GetAllPaths()
        {
            var response = await _service.GetAllPathsAsync();

            var dtos = response.Data?.Select(a => _mapper.Map<PathResponseDto>(a));
            return ResponseHelper.GetActionResult(response.StatusCode, dtos);
        }

        [HttpPut("{pathId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, 
            Roles = $"{Constants.Admin},{Constants.SuperChipper},{Constants.Chipper}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PathResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<ActionResult<PathResponseDto>> PutPath(
            [FromRoute] long? pathId, 
            [FromBody] PathRequestDto request)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == Constants.SuperChipper)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }

            var response = await _service.UpdatePathByIdAsync(pathId!.Value, request);

            var dto = _mapper.Map<PathResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto);
        }

        [HttpDelete("{pathId:long}")]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme, 
            Roles = $"{Constants.Admin},{Constants.SuperChipper},{Constants.Chipper}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> DeletePath([FromRoute] long? pathId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == Constants.SuperChipper)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }

            var statusCode = await _service.RemovePathByIdAsync(pathId!.Value);
            return ResponseHelper.GetActionResult(statusCode);
        }
    }
}
