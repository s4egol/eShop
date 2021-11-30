using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using OrderItemsReserver.Models;
using OrderItemsReserver.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrderItemsReserver.Services
{
    public class BlobService : IBlobService
    {
        private BlobServiceClient _blobClient;
        private OrderBusService _orderBusService;
        private readonly ILogger _log;

        private readonly int MaxAttemptCount = 2;

        public BlobService(ILogger log)
        {
            _log = log ?? throw new NullReferenceException(nameof(log));
            _orderBusService = new OrderBusService(_log);
        }

        public async Task<bool> TryUploadBlob(string containerName, Order order)
        {
            int attemp = default;
            bool responce = false;

            for (int i = 1; i <= MaxAttemptCount; i++)
            {
                attemp = i;

                try
                {
                    _blobClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=s4egolstorageaccount;AccountKey=qPUTFPTecDJ/6DYxcSCmaHqx0aszE4hupKl+01fNfAeSpY+11J/qXWQ4NiwruU0EoNZYMfZg6h7MBbJb9+7EIw==;EndpointSuffix=core.windows.net");
                    
                    responce = await UploadBlobAsync(containerName, order);

                    if (responce == true)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("Failed to upload blob");
                    }
                }
                catch (Exception ex)
                {
                    _log.LogInformation(ex.ToString());
                }
                finally
                {
                    if (attemp == MaxAttemptCount)
                    {
                        await _orderBusService.SendMessageInQueueAsync(order);
                    }
                }
            }

            return responce;
        }

        private async Task<bool> UploadBlobAsync(string containerName, Order order)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var orderKey = Guid.NewGuid().ToString();

            var blobClient = containerClient.GetBlobClient(orderKey);
            var responce = await blobClient.UploadAsync(new BinaryData(order));

            if (responce != null)
            {
                return true;
            }

            return false;
        }
    }
}
