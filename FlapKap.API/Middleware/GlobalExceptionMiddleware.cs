using FlapKap.Domain.Common;
using Serilog;
using System.Diagnostics;
using System.Net;

namespace FlapKap.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        Log.Error(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);
        
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        
        var response = Result.Failure($"Something went wrong with TraceId = {traceId}.");
        
        await context.Response.WriteAsJsonAsync(response);
    }
}