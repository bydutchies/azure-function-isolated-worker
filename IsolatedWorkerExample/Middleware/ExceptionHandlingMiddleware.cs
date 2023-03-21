using IsolatedWorkerExample.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IsolatedWorkerExample.Middleware;

/// <summary>
/// This middleware catches any exceptions during function invocations.
/// </summary>
internal sealed class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    try
    {
      await next(context);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Error processing invocation");

      var request = await context.GetHttpRequestDataAsync();
      if (request != null)
      {
        HttpResponseData response;
        if (e.GetType() == typeof(UnauthorizedAccessException))
        {
          // Unauthorized access
          response = request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        else if (e.GetType() == typeof(AggregateException) || e.GetType() == typeof(ArgumentException) || e.GetType().IsSubclassOf(typeof(ArgumentException)))
        {
          response = request.CreateResponse(HttpStatusCode.UnprocessableEntity);
          await response.WriteAsJsonAsync(new UnprocessableEntityResponse
          {
            Referentie = new Guid(context.CorrelationId()),
            Message = e.Message
          }, response.StatusCode); // Statuscode will be set to 200 if not set here
        }
        else
        {
          // Unknown exception
          response = request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        context.InvokeResult(response);
        return;
      }

      throw;
    }
  }
}
