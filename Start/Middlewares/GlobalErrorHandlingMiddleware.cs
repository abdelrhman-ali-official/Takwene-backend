using Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.ErrorModels;
using System.Net;

namespace Takwene.Api.Middlewares;

public class GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
            if (httpContext.Response.StatusCode == (int)HttpStatusCode.NotFound && !httpContext.Response.HasStarted)
                await WriteErrorAsync(httpContext, HttpStatusCode.NotFound, "The requested endpoint was not found.");
        }
        catch (OperationCanceledException) when (httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Request {TraceIdentifier} was cancelled.", httpContext.TraceIdentifier);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception for request {TraceIdentifier}.", httpContext.TraceIdentifier);
            var (statusCode, message) = exception switch
            {
                ValidationException => (HttpStatusCode.BadRequest, "The request is invalid."),
                UnAuthorizedException or UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Authentication failed."),
                EntityNotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found."),
                ArtistHasTracksException => (HttpStatusCode.Conflict, exception.Message),
                ConflictException => (HttpStatusCode.Conflict, "The request conflicts with existing data."),
                DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 or 547 } } => (HttpStatusCode.Conflict, "The request conflicts with existing data."),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };
            await WriteErrorAsync(httpContext, statusCode, message);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = (int)statusCode,
            ErrorMessage = message
        }.ToString());
    }
}
