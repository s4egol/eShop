using DeliveryOrderProcessor.Models;
using DeliveryOrderProcessor.Services.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrderProcessor.Services
{
    public class OrderService : IOrderService
    {
        private readonly CosmosClient CosmosClient;
        private readonly Database Database;
        private readonly Container Container;

        private readonly string EndpointUri = "https://s4egol.documents.azure.com:443/";
        private readonly string PrimaryKey = "ZGjrTLM1HdtJpjHjKHnW4ear7ArYqUUB3Q1xXMMCR3gHzNVuxHxdgAHnI2t6XZxYrxFy6JMnmoCASCZw9E1kpA==";
        private readonly string DatabaseId = "Shop";
        private readonly string ContainerId = "Orders";

        public OrderService()
        {
            CosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "CosmosDBDotnetQuickstart" });
            Database = CosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId).Result;
            Container = Database.CreateContainerIfNotExistsAsync(ContainerId, "/pk").Result;
        }

        public async Task AddItemsToContainerAsync(Order order)
        {
            await ScaleContainerAsync();

            order.PartitionKey = order.Address.Country;

            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Order> andersenFamilyResponse = await Container.ReadItemAsync<Order>(order.Id, new PartitionKey(order.PartitionKey));
            }
            catch
            {
                ItemResponse<Order> andersenFamilyResponse = await Container.CreateItemAsync(order, new PartitionKey(order.PartitionKey));
            }
        }

        private async Task ScaleContainerAsync()
        {
            int? throughput = await Container.ReadThroughputAsync();

            if (throughput.HasValue)
            {
                int newThroughput = throughput.Value + 100;

                // Update throughput
                var responce = await Container.ReplaceThroughputAsync(newThroughput);
            }
        }
    }
}
