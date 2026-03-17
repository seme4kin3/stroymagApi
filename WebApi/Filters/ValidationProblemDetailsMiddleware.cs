using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Text.Encodings.Web;
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

                await WriteJsonAsync(context, HttpStatusCode.BadRequest, new
                {
                    error = "ValidationFailed",
                    details = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
            catch (NotFoundException ex)
            {
                logger.LogInformation(ex, "Not found");

                await WriteJsonAsync(context, HttpStatusCode.NotFound, new
                {
                    error = "NotFound",
                    message = ex.Message
                });
            }
            catch (ConflictException ex)
            {
                logger.LogInformation(ex, "Conflict");

                await WriteJsonAsync(context, HttpStatusCode.Conflict, new
                {
                    error = "Conflict",
                    message = ex.Message
                });
            }
            catch (DomainException ex)
            {
                logger.LogInformation(ex, "Domain error");

                await WriteJsonAsync(context, HttpStatusCode.UnprocessableEntity, new
                {
                    error = "DomainError",
                    message = ex.Message
                });
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
            {
                logger.LogWarning(ex, "Unique constraint violation");

                await WriteJsonAsync(context, HttpStatusCode.Conflict, new
                {
                    error = "Conflict",
                    message = "Запись с такими данными уже существует."
                });
            }
        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static async Task WriteJsonAsync(HttpContext context, HttpStatusCode statusCode, object payload)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, _jsonOptions));
        }
    }
}
