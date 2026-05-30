using System.Net;
using System.Text.Json;

namespace ClickExpress.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized: {Path}", context.Request.Path);
                await WriteError(context, HttpStatusCode.Unauthorized, "Unauthorized");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found: {Path}", context.Request.Path);
                await WriteError(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request: {Path}", context.Request.Path);
                await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception at {Method} {Path}", context.Request.Method, context.Request.Path);
                var message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred";
                await WriteError(context, HttpStatusCode.InternalServerError, message);
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode status, string message)
        {
            if (context.Response.HasStarted) return;

            var correlationId = context.Items.TryGetValue("CorrelationId", out var cid)
                ? cid?.ToString()
                : null;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var payload = JsonSerializer.Serialize(new
            {
                statusCode = (int)status,
                message,
                correlationId
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(payload);
        }
    }
}
