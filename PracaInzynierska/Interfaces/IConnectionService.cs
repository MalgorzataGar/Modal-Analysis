using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using PracaInzynierska.Models;

namespace PracaInzynierska.Interfaces
{
    public interface IConnectionService

    {
        Task HandleConnection(JObject json);
        Task<string> PostRequest(HttpContent content, HttpClient client);
        RecaivedData GetDataFromString();
    }
}
