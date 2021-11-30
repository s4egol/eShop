using OrderItemsReserver.Models;
using System.Threading.Tasks;

namespace OrderItemsReserver.Services.Interfaces
{
    public interface IOrderBusService
    {
        Task SendMessageInQueueAsync(Order order);
    }
}
