using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Dtos.LocationPath;
using Olymp_Project.Dtos.ShortestPath;
using Olymp_Project.Helpers;
using Olymp_Project.Services.LocationsPaths;

namespace Olymp_Project.Controllers
{
    /// <summary>
    /// A hypothetical controller to implement in the 3rd stage of the contest.
    /// </summary>
    [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme), AllowAnonymous]
    [Route("locations/{locationIdFrom}")]
    [ApiController]
    public class ShortestPathController : ControllerBase
    {
        private readonly IShortestPathService _service;

        public ShortestPathController(IShortestPathService service)
        {
            _service = service;
        }

        [HttpGet("best-path-with-dijkstra")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DijkstraDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DijkstraDto>> GetBestPathWithDijkstra(
            [FromRoute] long? locationIdFrom,
            [FromQuery] long? locationIdTo)
        {
            var response = await _service.FindShortestPathWithDijkstraAsync(locationIdFrom, locationIdTo);
            return ResponseHelper.GetActionResult(response.StatusCode, response.Data);
        }

        [HttpGet("best-path-with-bellman-ford")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BellmanFordDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BellmanFordDto>> GetBestPathWithBellmanFord(
            [FromRoute] long? locationIdFrom,
            [FromQuery] long? locationIdTo)
        {
            var response = await _service.FindShortestPathWithBellmanFordAsync(locationIdFrom, locationIdTo);
            return ResponseHelper.GetActionResult(response.StatusCode, response.Data);
        }
    }
}
