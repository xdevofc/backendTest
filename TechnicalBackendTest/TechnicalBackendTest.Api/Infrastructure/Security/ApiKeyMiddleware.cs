namespace TechnicalBackendTest.Api.Infrastructure.Security;

public sealed class ApiKeyMiddleware
{
    private const string ApiKeyHeaderName = "X-API-KEY";
    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["Security:ApiKey"]
            ?? throw new InvalidOperationException("Security:ApiKey must be configured.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValue))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var apiKey = apiKeyHeaderValue.ToString();

        if (string.IsNullOrWhiteSpace(apiKey) ||
            !string.Equals(apiKey, _apiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
