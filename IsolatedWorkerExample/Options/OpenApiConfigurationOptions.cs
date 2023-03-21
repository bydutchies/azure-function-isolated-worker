using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace IsolatedWorkerExample.Options;

internal class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
  public override OpenApiInfo Info { get; set; } = new OpenApiInfo
  {
    Version = "1.0.0",
    Title = "Azure Function Isolated Worker example"
  };

  public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
}
