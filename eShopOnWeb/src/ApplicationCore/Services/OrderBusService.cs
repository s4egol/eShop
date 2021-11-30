using Microsoft.Azure.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class OrderBusService : IOrderBusService
    {
        private readonly IAppLogger<OrderBusService> _logger;

        private readonly string ServiceBusConnectionString = "Endpoint=sb://s4egol.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8mdNJ3YwqloBTdEHrVQENWK8hfBKNQx0zcpbtPz0wrM=";
        private readonly string ServiceBusQueueName = "orders";

        public OrderBusService(IAppLogger<OrderBusService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendMessageInQueueAsync(Entities.OrderApi.Order order)
        {
            var queueClient = new QueueClient(ServiceBusConnectionString, ServiceBusQueueName);

            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(order.ToJson()));
                await queueClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            finally
            {
                await queueClient?.CloseAsync();
            }
        }
    }
}
