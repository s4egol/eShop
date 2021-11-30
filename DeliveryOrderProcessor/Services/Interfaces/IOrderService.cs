using DeliveryOrderProcessor.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrderProcessor.Services.Interfaces
{
    public interface IOrderService
    {
        Task AddItemsToContainerAsync(Order order);
    }
}
