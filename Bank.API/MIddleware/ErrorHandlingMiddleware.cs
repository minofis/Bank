using System.Text.Json;
using Bank.Core.Exceptions;

namespace Bank.API.MIddleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/json";

                switch (ex)
                {
                    case ArgumentException argEx:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = argEx.Message }));
                        break;

                    case NotFoundException notFoundEx:
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = notFoundEx.Message }));
                        break;

                    case InsufficientFundsException insufficientFundsEx:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = insufficientFundsEx.Message }));
                        break;

                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Internal server error" }));
                        break;
                }
            }
        }
    }
}