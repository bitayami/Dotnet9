using System.Net;
using System.Text.Json;

namespace Dotnet9.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) {
                _logger.LogError(ex,ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            //var errorResponse = new
            //{
            //    Message = ex.Message,
            //    StatusCode = (int)HttpStatusCode.InternalServerError,
            //    TraceId = context.TraceIdentifier,
            //};


            int statusCode;
            string message;

            switch (ex)
            {
                case ArgumentNullException:
                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = ex.Message;
                    break;
                case InvalidOperationException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = ex.Message;
                    break;
                case TimeoutException:
                    statusCode = StatusCodes.Status408RequestTimeout;
                    message = ex.Message;
                    break;
                case FormatException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case DivideByZeroException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = ex.Message;
                    break;
                case NullReferenceException:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = ex.Message;
                    break;
                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = ex.Message;
                    break;
            }

            var errorResponse = new
            {
                Message = message,
                StatusCode = statusCode,
                TraceId = context.TraceIdentifier,
            };


            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
