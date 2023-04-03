using Microsoft.AspNetCore.Mvc;

namespace Olymp_Project.Helpers
{
    public static class ResponseHelper
    {
        public static ActionResult GetActionResult(
            HttpStatusCode statusCode, object? value = null, string? actionName = "")
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
                    return new BadRequestResult();
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedResult();
                case HttpStatusCode.Forbidden:
                    return new ForbidResult(ApiAuthenticationScheme.Name);
                case HttpStatusCode.NotFound:
                    return new NotFoundResult();
                case HttpStatusCode.Conflict:
                    return new ConflictResult();
                default:
                    return new StatusCodeResult(500);
            }
        }

        // TODO: remove?
        public static ActionResult GetTestActionResult(
            HttpStatusCode statusCode, object? value = null, string? actionName = "", string? errorMessage = "")
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
                    return new BadRequestResult();
                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedResult();
                case HttpStatusCode.Forbidden:
                    return new ForbidResult(ApiAuthenticationScheme.Name);
                case HttpStatusCode.NotFound:
                    return new NotFoundResult();
                case HttpStatusCode.Conflict:
                    return new ConflictResult();
                default:
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        return new ObjectResult(errorMessage) { StatusCode = 500 };
                    }
                    else
                    {
                        return new StatusCodeResult(500);
                    }
            }
        }
    }
}
