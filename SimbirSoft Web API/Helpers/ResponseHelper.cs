﻿using Microsoft.AspNetCore.Mvc;

namespace SimbirSoft_Web_API.Helpers;

public static class ResponseHelper
{
    public static ActionResult GetActionResult(
        HttpStatusCode statusCode,
        object? value = null,
        string? actionName = "",
        string? errorMessage = "")
    {
        switch (statusCode)
        {
            case HttpStatusCode.OK:
                if (value is not null)
                {
                    return new OkObjectResult(value);
                }
                else
                {
                    return new StatusCodeResult(200);
                }
            case HttpStatusCode.Created:
                return new CreatedAtActionResult(actionName, null, null, value);
            case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(errorMessage);
            case HttpStatusCode.Unauthorized:
                return new UnauthorizedResult();
            case HttpStatusCode.Forbidden:
                return new ForbidResult(Constants.BasicAuthScheme);
            case HttpStatusCode.NotFound:
                return new NotFoundResult();
            case HttpStatusCode.Conflict:
                return new ConflictResult();
            default:
                return new ObjectResult(errorMessage) { StatusCode = 500 };
        }
    }
}
