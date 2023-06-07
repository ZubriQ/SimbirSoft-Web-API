using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimbirSoft_Web_API.Helpers;
using SimbirSoft_Web_API.Services.AreaAnalytics;

namespace SimbirSoft_Web_API.Controllers;

[Authorize(AuthenticationSchemes = Constants.BasicAuthScheme)]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AreaAnalyticsResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AreaAnalyticsResponseDto>> GetAreaAnalytics(
        [FromRoute] long? areaId,
        [FromQuery] AreaAnalyticsQuery query)
    {
        var response = await _service.GetAnalyticsByAreaIdAsync(areaId!.Value, query);
        return ResponseHelper.GetActionResult(response.StatusCode, response.Data);
    }
}
