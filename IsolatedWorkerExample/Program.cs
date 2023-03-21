using IsolatedWorkerExample.Middleware;
using IsolatedWorkerExample.Services;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IsolatedWorkerExample;

public class Program
{
  public static void Main()
  {
    var host = new HostBuilder()
      .ConfigureAppConfiguration(builder =>
      {
        var environmentSection = $".{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}";

        builder.AddJsonFile($"appsettings{environmentSection}.json", optional: false, reloadOnChange: false);
        builder.AddEnvironmentVariables();
      })
      .ConfigureOpenApi()
      .ConfigureFunctionsWorkerDefaults(app =>
      {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseWhen<SubscriptionMiddleware>(functionContext =>
        {
          // Only use the middleware if not related to swagger
          return !functionContext.FunctionDefinition.Name.Contains("Swagger");
        });
        app.UseMiddleware<CorrelationMiddleware>();
      })
      .ConfigureServices((context, services) =>
      {
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        services.Configure<JsonSerializerOptions>(options =>
        {
          options.AllowTrailingCommas = true;
          options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
          options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
          options.PropertyNameCaseInsensitive = true;
        });
      })
      .Build();

    host.Run();
  }
}
