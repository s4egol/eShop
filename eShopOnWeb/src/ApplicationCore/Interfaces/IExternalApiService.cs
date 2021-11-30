using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IExternalApiService
    {
        Task RunRequest<TRequest>(string url, TRequest request, Dictionary<string, string> headers);
    }
}
