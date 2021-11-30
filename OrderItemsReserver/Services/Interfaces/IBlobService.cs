using OrderItemsReserver.Models;
using System.Threading.Tasks;

namespace OrderItemsReserver.Services.Interfaces
{
    public interface IBlobService
    {
        Task<bool> TryUploadBlob(string containerName, Order order);
    }
}
