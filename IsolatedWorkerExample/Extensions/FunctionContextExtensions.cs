using Microsoft.Azure.Functions.Worker.Http;

namespace Microsoft.Azure.Functions.Worker;

internal static class FunctionContextExtensions
{
  internal static void InvokeResult(this FunctionContext context, HttpResponseData response)
  {
    var keyValuePair = context.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
    var functionBindingsFeature = keyValuePair.Value;
    var type = functionBindingsFeature.GetType();
    var result = type.GetProperties().Single(p => p.Name == "InvocationResult");
    result.SetValue(functionBindingsFeature, response);
  }

  public static string CorrelationId(this FunctionContext context)
  {
    return (string)context.Items["correlationId"];
  }

  public static int OrganisationId(this FunctionContext context)
  {
    return (int)context.Items["organisationId"];
  }
}
