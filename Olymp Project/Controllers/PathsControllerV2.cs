using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Dtos.Path;
using Olymp_Project.Helpers;
using Olymp_Project.Services.PathsV2;

namespace Olymp_Project.Controllers
{
    /// <summary>
    /// A hypothetical controller to implement in the 3rd stage of the contest.
    /// </summary>
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme), AllowAnonymous]
    [Route("hypothetical-stage3-endpoint/pathsV2")]
    [ApiController]
    public class PathsV2Controller : ControllerBase
    {
        private readonly IPathV2Service _service;
        private readonly IMapper _mapper;

        public PathsV2Controller(IPathV2Service service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PathResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PathResponseDto>> CreatePath([FromBody] PathRequestDto request)
        {
            var response = await _service.InsertPathAsync(request);

            var dto = _mapper.Map<PathResponseDto>(response.Data);
            return ResponseHelper.GetActionResult(response.StatusCode, dto, nameof(CreatePath));
        }
    }
}
