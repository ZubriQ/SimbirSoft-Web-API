using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Olymp_Project.Helpers;
using Olymp_Project.Services.AreaAnalytics;
using Olymp_Project.Services.Authentication;

namespace Olymp_Project.Controllers
{
    [Authorize]
    [Route("areas/{areaId}/analytics")]
    [ApiController]
    public class AreaAnalyticsController : ControllerBase
    {
        private readonly IAreaAnalyticsService _service;

        public AreaAnalyticsController(IAreaAnalyticsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AreaResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AreaResponseDto>> GetArea(
            [FromRoute] long? areaId,
            [FromQuery] AreaAnalyticsQuery query)
        {
            if (!await ApiAuthentication.IsAuthorizationValid(Request, HttpContext))
            {
                return Unauthorized();
            }

            var response = await _service.GetAnalyticsByAreaIdAsync(areaId!.Value, query);
            
            return ResponseHelper.GetActionResult(
                response.StatusCode, response.Data);
        }
    }
}
