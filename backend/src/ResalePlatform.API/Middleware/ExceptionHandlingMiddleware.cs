using System.Net;
using ResalePlatform.Application.Common.Exceptions;
using ValidationException = ResalePlatform.Application.Common.Exceptions.ValidationException;

namespace ResalePlatform.API.Middleware;

/// <summary>
/// Ловит доменные исключения и превращает их в аккуратные ProblemDetails-ответы.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Ошибка валидации"),
            ConflictException => (HttpStatusCode.Conflict, "Конфликт"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Не авторизовано"),
            NotFoundException => (HttpStatusCode.NotFound, "Не найдено"),
            _ => (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера"),
        };

        if (status == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Необработанное исключение");

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = $"https://httpstatuses.io/{(int)status}",
            title,
            status = (int)status,
            detail = status == HttpStatusCode.InternalServerError ? "Что-то пошло не так." : ex.Message,
            errors = ex is ValidationException v ? v.Errors : null,
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
