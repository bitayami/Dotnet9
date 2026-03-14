using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
            Exception ex, 
            CancellationToken cancellationToken)
        {
            _logger.LogError(ex, ex.Message);

            var problemDetails = new ProblemDetails();

            switch (ex)
            {
                case ArgumentNullException:
                case ArgumentException:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = ex.Message;
                    break;
                case UnauthorizedAccessException:
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Detail = ex.Message;
                    break;
                case InvalidOperationException:
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Detail = ex.Message;
                    break;
                case TimeoutException:
                    problemDetails.Status = StatusCodes.Status408RequestTimeout;
                    problemDetails.Detail = ex.Message;
                    break;
                case FormatException:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = ex.Message;
                    break;
                case DivideByZeroException:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Detail = ex.Message;
                    break;
                case NullReferenceException:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Detail = ex.Message;
                    break;
                case KeyNotFoundException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Detail = ex.Message;
                    break;
                case NotFoundException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Detail = ex.Message;
                    break;
                case ForbiddenException:
                    problemDetails.Status = StatusCodes.Status403Forbidden;
                    problemDetails.Detail = ex.Message;
                    break;
                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Detail = ex.Message;
                    break;
            }

            problemDetails.Instance = httpContext.Request.Path;
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails,cancellationToken);

            return true;
        }
    }
}
