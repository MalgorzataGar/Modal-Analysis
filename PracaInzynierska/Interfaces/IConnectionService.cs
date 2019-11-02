using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace PracaInzynierska.Interfaces
{
    interface IConnectionService

    {
        Task HandleConnection(JObject json);
        Task<string> PostRequest(HttpContent content, HttpClient client);
    }
}
