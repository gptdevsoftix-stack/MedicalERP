using FluentValidation;
using MedicalERP.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MedicalERP.Web.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    ITempDataDictionaryFactory tempDataFactory)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled request failure");
            var (status, title) = ex switch
            {
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                ValidationException => (StatusCodes.Status400BadRequest, "Validation Failed"),
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency Conflict"),
                ConcurrencyConflictException => (StatusCodes.Status409Conflict, "Concurrency Conflict"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid Operation"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error")
            };
            var problem = new ProblemDetails { Status = status, Title = title, Detail = ex.Message, Instance = context.Request.Path };

            if (!context.Response.HasStarted && IsBrowserWrite(context.Request))
            {
                tempDataFactory.GetTempData(context)["ErrorMessage"] = ex.Message;
                context.Response.Redirect(GetSafeReturnUrl(context));
                return;
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }

    private static bool IsBrowserWrite(HttpRequest request)
    {
        var isWrite = HttpMethods.IsPost(request.Method) || HttpMethods.IsPut(request.Method)
            || HttpMethods.IsPatch(request.Method) || HttpMethods.IsDelete(request.Method);
        var acceptsHtml = request.GetTypedHeaders().Accept?.Any(x =>
            string.Equals(x.MediaType.Value, "text/html", StringComparison.OrdinalIgnoreCase)) == true;
        var isAjax = string.Equals(request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        return isWrite && acceptsHtml && !isAjax;
    }

    private static string GetSafeReturnUrl(HttpContext context)
    {
        var referer = context.Request.GetTypedHeaders().Referer;
        if (referer is not null && string.Equals(referer.Host, context.Request.Host.Host, StringComparison.OrdinalIgnoreCase))
        {
            return referer.PathAndQuery;
        }
        return "/";
    }
}

