using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly IAppLogger<ExternalApiService> _logger;

        public ExternalApiService()
        {

        }

        public async Task RunRequest<TRequest>(string url, TRequest request, Dictionary<string, string> headers)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(request?.ToJson(), Encoding.UTF8, "application/json")
                };

                if (headers != null && headers.Any())
                {
                    AddHeaders(httpRequest, headers);
                }

                using var httpClient = new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                });

                await httpClient.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString(), $"Failed to load data. Url: {url}");

                if (ex is WebException webException)
                {
                    var status = (webException.Response as HttpWebResponse)?.StatusCode;

                    if (status == HttpStatusCode.Forbidden)
                    {
                        throw new UnauthorizedAccessException("UnauthorizedAccessException");
                    }

                    var responseStream = webException.Response?.GetResponseStream();
                    var errorResponse = responseStream != null ? await new StreamReader(responseStream).ReadToEndAsync() : string.Empty;

                    _logger.LogInformation(errorResponse);
                }

                throw;
            }
        }

        private void AddHeaders(HttpRequestMessage httpRequest, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                if (!string.IsNullOrWhiteSpace(header.Value))
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}
