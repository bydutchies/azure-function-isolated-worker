using IsolatedWorkerExample.Models;

namespace IsolatedWorkerExample.Services;

#region Interface
internal interface IEmployeeService
{
  Task<Employee> GetEmployeeAsync(int organisationId, int id);
}
#endregion Interface

internal class EmployeeService : IEmployeeService
{
  public async Task<Employee> GetEmployeeAsync(int organisationId, int id)
  {
    return await Task.FromResult(new Employee
    {
      Id = id,
      OrganisationId = organisationId,
      Name = "Jack",
      Age = 25,
      Job = "Software Engineer"
    });
  }
}
