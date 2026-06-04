using System.Diagnostics;
using System.Security.Claims;

namespace ClickExpress.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private static readonly HashSet<string> _skipPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/health/live", "/health/ready", "/favicon.ico"
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_skipPaths.Contains(context.Request.Path.Value ?? string.Empty))
            {
                await _next(context);
                return;
            }

            var sw = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();

                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User?.FindFirst("sub")?.Value
                          ?? "anon";

                var correlationId = context.Items.TryGetValue("CorrelationId", out var cid)
                    ? cid?.ToString()
                    : null;

                var status = context.Response.StatusCode;
                var level = status >= 500 ? LogLevel.Error
                          : status >= 400 ? LogLevel.Warning
                          : LogLevel.Information;

                _logger.Log(
                    level,
                    "HTTP {Method} {Path} → {Status} in {Ms}ms  uid={UserId}  cid={CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    status,
                    sw.ElapsedMilliseconds,
                    userId,
                    correlationId ?? "-");
            }
        }
    }
}
