using System.Net;
using ChatApp.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Validation error", ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await WriteProblemDetails(context, HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemDetails(context, HttpStatusCode.NotFound, "Not found", ex.Message);
        }
        catch (Exception ex)
        {
            await WriteProblemDetails(context, HttpStatusCode.InternalServerError, "Server error", ex.Message);
        }
    }

    private static async Task WriteProblemDetails(HttpContext context, HttpStatusCode status, string title, string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;

        var problem = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}