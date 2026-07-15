using System.Net;

namespace WeatherAPI.Middleware
{
    public class Exceptionsm
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Exceptionsm> _logger;

        public Exceptionsm(RequestDelegate next, ILogger<Exceptionsm> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.TraceIdentifier;

            context.Response.Headers["X-Correlation-ID"] = correlationId;

            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteError(context, HttpStatusCode.NotFound, ex.Message, correlationId);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout na API externa.");

                await WriteError(context, HttpStatusCode.BadGateway,"O serviço externo demorou demasiado a responder.", correlationId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro na API externa.");

                await WriteError(context, HttpStatusCode.BadGateway, "Não foi possível consultar o serviço meteorológico.", correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Erro inesperado. Correlation ID: {CorrelationId}", correlationId);

                await WriteError(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno.", correlationId);
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message, string correlationId)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message,correlationId
            });
        }
    }
}
