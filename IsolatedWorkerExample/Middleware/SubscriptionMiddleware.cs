using IsolatedWorkerExample.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace IsolatedWorkerExample.Middleware;

internal class SubscriptionMiddleware : IFunctionsWorkerMiddleware
{
  private readonly ISubscriptionService _service;

  public SubscriptionMiddleware(ISubscriptionService service)
  {
    _service = service;
  }

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    // Is Http trigger invoked?
    var httpRequestData = await context.GetHttpRequestDataAsync();
    if (httpRequestData != null)
    {
      if (httpRequestData.Headers.TryGetValues("x-api-key", out var apiKeyHeaders))
      {
        var apiKey = apiKeyHeaders.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
          throw new UnauthorizedAccessException();

        var subscription = await _service.GetSubscriptionAsync(apiKey);
        if (subscription == null)
          throw new UnauthorizedAccessException();

        // Make context available to functions via FunctionContext
        context.Items.Add("organisationId", subscription.OrganisationId);
      }
      else
      {
        // Api key header missing
        throw new UnauthorizedAccessException();
      }
    }

    // Continue
    await next(context).ConfigureAwait(false);
  }
}