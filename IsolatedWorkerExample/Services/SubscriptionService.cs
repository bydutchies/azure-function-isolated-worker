using IsolatedWorkerExample.Models;

namespace IsolatedWorkerExample.Services;

#region Interface
internal interface ISubscriptionService
{
  Task<Subscription> GetSubscriptionAsync(string apiKey);
}
#endregion Interface

internal class SubscriptionService : ISubscriptionService
{
  public async Task<Subscription> GetSubscriptionAsync(string apiKey)
  {
    return await Task.FromResult(new Subscription
    {
      ApiKey = apiKey,
      OrganisationId = 1
    });
  }
}
