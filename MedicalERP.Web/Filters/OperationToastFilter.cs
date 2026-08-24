using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedicalERP.Web.Filters;

/// <summary>Adds consistent feedback for successful browser-based write operations.</summary>
public sealed class OperationToastFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var isWrite = HttpMethods.IsPost(request.Method)
            || HttpMethods.IsPut(request.Method)
            || HttpMethods.IsPatch(request.Method)
            || HttpMethods.IsDelete(request.Method);
        var isPageResult = context.Result is RedirectResult
            or RedirectToActionResult
            or RedirectToRouteResult;
        var controller = context.Controller as Controller;

        if (controller is not null && isWrite && isPageResult && !controller.ViewData.ModelState.IsValid)
        {
            controller.TempData["ErrorMessage"] ??= "The action could not be completed. Please correct the highlighted fields.";
        }
        else if (controller is not null && isWrite && isPageResult)
        {
            controller.TempData["SuccessMessage"] ??= BuildSuccessMessage(context);
        }

        await next();
    }

    private static string BuildSuccessMessage(ResultExecutingContext context)
    {
        var controller = context.ActionDescriptor.RouteValues["controller"] ?? "Record";
        var action = context.ActionDescriptor.RouteValues["action"] ?? "Save";
        var subject = SplitWords(controller.EndsWith("ies", StringComparison.OrdinalIgnoreCase)
            ? controller[..^3] + "y"
            : controller.EndsWith('s') ? controller[..^1] : controller);

        var operation = action.Contains("delete", StringComparison.OrdinalIgnoreCase)
            || action.Contains("deactivate", StringComparison.OrdinalIgnoreCase) ? "deleted"
            : action.Contains("create", StringComparison.OrdinalIgnoreCase) ? "created"
            : action.Contains("edit", StringComparison.OrdinalIgnoreCase)
                || action.Contains("update", StringComparison.OrdinalIgnoreCase) ? "updated"
            : "saved";

        return $"{subject} {operation} successfully.";
    }

    private static string SplitWords(string value) =>
        System.Text.RegularExpressions.Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
}
