using IsolatedWorkerExample.Models;
using IsolatedWorkerExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Net;

namespace IsolatedWorkerExample.Endpoints.Customers;

internal class Get
{
  private readonly IEmployeeService _service;

  public Get(IEmployeeService service)
  {
    _service = service;
  }

  [OpenApiOperation(operationId: "GetEmployee", tags: new[] { "Employees" }, Summary = "Get employee")]
  [OpenApiSecurity("ApiKey", SecuritySchemeType.ApiKey, Name = "x-api-key", In = OpenApiSecurityLocationType.Header)]
  [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Employee), Description = "Ok")]
  [OpenApiResponseWithBody(statusCode: HttpStatusCode.UnprocessableEntity, contentType: "application/json", bodyType: typeof(UnprocessableEntityResponse), Description = "Unprocessable Entity")]
  [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Unauthorized, Description = "Unauthorized")]
  [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Not Found")]
  [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
  [OpenApiParameter("id", Description = "Employee id", Required = true)]
  [Function("GetEmployee")]
  public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/employees/{id}")] HttpRequestData req,
    int id,
    FunctionContext context)
  {
    var customer = await _service.GetEmployeeAsync(context.OrganisationId(), id);
    if (customer == null)
      return req.CreateResponse(HttpStatusCode.NotFound);

    // Bug: Throwing an (Argument)Exception from an async function always results in an
    // AggregateException in ExceptionHandlingMiddleware. This is an known issue and
    // will be solved in next major release.
    //throw new ArgumentException("This employee is no longer employed.");  

    var response = req.CreateResponse(HttpStatusCode.OK);
    await response.WriteAsJsonAsync(customer, response.StatusCode);
    return response;
  }
}
