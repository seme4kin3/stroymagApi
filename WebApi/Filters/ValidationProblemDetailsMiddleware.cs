using FluentValidation;
using System.Net;
using System.Text.Json;

namespace WebApi.Filters
{
    public sealed class ValidationProblemDetailsMiddleware(RequestDelegate next, ILogger<ValidationProblemDetailsMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, "Validation failed");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    error = "ValidationFailed",
                    details = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}
