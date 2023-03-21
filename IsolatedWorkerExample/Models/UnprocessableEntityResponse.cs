namespace IsolatedWorkerExample.Models;

internal class UnprocessableEntityResponse
{
  public Guid Referentie { get; set; }
  public string Message { get; set; }
}
