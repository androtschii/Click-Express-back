namespace ClickExpress.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                ?? Guid.NewGuid().ToString("N");

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            using var scope = context.RequestServices
                .GetRequiredService<ILogger<CorrelationIdMiddleware>>()
                .BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId });

            await _next(context);
        }
    }
}
