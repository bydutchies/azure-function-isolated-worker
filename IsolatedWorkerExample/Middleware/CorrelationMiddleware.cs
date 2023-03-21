using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace IsolatedWorkerExample.Middleware;

internal class CorrelationMiddleware : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var requestData = await context.GetHttpRequestDataAsync();

    string correlationId;
    if (requestData!.Headers.TryGetValues("x-correlation-id", out var values))
    {
      correlationId = values.First();
    }
    else
    {
      correlationId = Guid.NewGuid().ToString();
    }
    context.Items.Add("correlationId", correlationId);

    // Continue
    await next(context);

    // Modify response
    context.GetHttpResponseData()?.Headers.Add("x-correlation-id", correlationId);
  }
}