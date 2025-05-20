using System.Threading.RateLimiting;
using WebHookDynamics.DTO;

namespace WebHookDynamics.DynamicsFacade.Interfaces;

public interface IDynamicsWebhookLogic
{
     Task ProcessOrder(DynamicsOrderDto dto);
     
     Task CreateCustomer(CreateCustomerDto dto);
}