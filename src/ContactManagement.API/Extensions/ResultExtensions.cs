using ContactManagement.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                return controller.Ok(result.Value);
            }

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Detail = result.Error?.Message ?? "An error occurred",
                Status = (int)result.StatusCode,
                Instance = controller.HttpContext.Request.Path
            };

            problemDetails.Extensions.Add("errorCode", result.Error?.Code ?? "Unknown");

            return new ObjectResult(problemDetails)
            {
                StatusCode = (int)result.StatusCode
            };
        }

        public static ActionResult ToCreatedResult<T>(
            this Result<T> result, 
            ControllerBase controller, 
            string actionName, 
            object routeValues, 
            T value)
        {
            if (result.IsSuccess)
            {
                return controller.CreatedAtAction(actionName, routeValues, value);
            }

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Detail = result.Error?.Message ?? "An error occurred",
                Status = (int)result.StatusCode,
                Instance = controller.HttpContext.Request.Path
            };

            problemDetails.Extensions.Add("errorCode", result.Error?.Code ?? "Unknown");

            return new ObjectResult(problemDetails)
            {
                StatusCode = (int)result.StatusCode
            };
        }
    }
}

