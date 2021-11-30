using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DeliveryOrderProcessor.Models;
using DeliveryOrderProcessor.Services;

namespace DeliveryOrderProcessor
{
    public static class DeliveryOrderProcessor
    {
        [FunctionName(nameof(DeliveryOrderProcessor))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            await new OrderService().AddItemsToContainerAsync(order);

            return new OkResult();
        }
    }
}
