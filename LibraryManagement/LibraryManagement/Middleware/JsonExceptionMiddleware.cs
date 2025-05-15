using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Middleware
{
    public class JsonExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JsonExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public JsonExceptionMiddleware(
            RequestDelegate next,
            ILogger<JsonExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parsing error");
                await HandleExceptionAsync(context, jsonEx, "Invalid JSON format", StatusCodes.Status400BadRequest);
            }
            catch (KeyNotFoundException notFoundEx)
            {
                _logger.LogWarning(notFoundEx, "Resource not found");
                await HandleExceptionAsync(context, notFoundEx, "Resource not found", StatusCodes.Status404NotFound);
            }
            catch (UnauthorizedAccessException unauthorizedEx)
            {
                _logger.LogWarning(unauthorizedEx, "Unauthorized access attempt");
                await HandleExceptionAsync(context, unauthorizedEx, "Unauthorized access", StatusCodes.Status401Unauthorized);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Invalid operation");
                await HandleExceptionAsync(context, invOpEx, "Invalid operation", StatusCodes.Status400BadRequest);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument");
                await HandleExceptionAsync(context, argEx, "Invalid argument", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                await HandleExceptionAsync(context, ex, "Internal Server Error", StatusCodes.Status500InternalServerError);
            }


        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string title, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = title,
                status = statusCode,
                detail = exception.Message,
                traceId = context.TraceIdentifier
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}