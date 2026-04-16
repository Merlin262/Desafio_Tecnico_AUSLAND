using ProductsAPI.API.Exceptions;
using System.Text.Json;

namespace ProductsAPI.API.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ApiException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                var body = JsonSerializer.Serialize(new { message = ex.Message });
                await context.Response.WriteAsync(body);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var body = JsonSerializer.Serialize(new { message = "Erro interno no servidor." });
                await context.Response.WriteAsync(body);
            }
        }
    }
}
