using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Models;
using OrderItemsReserver.Services;

namespace OrderItemsReserver
{
    public static class OrderItemsReserver
    {
        [FunctionName(nameof(OrderItemsReserver))]
        public static void Run([ServiceBusTrigger("orders", Connection = "RemoteServiceBusConnectionString")]string myQueueItem, ILogger log)
        {
            var order = JsonConvert.DeserializeObject<Order>(myQueueItem);
            var blobService = new BlobService(log);

            try
            {
                var result = blobService.TryUploadBlob("orders", order).Result;
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
            }
        }
    }
}
