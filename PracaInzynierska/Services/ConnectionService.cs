using System;
using System.Threading.Tasks;
using PracaInzynierska.Interfaces;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using PracaInzynierska.Models;

namespace PracaInzynierska.Services
{

    public class ConnectionService : IConnectionService

    {
        public string myUrl;
        public string contextResponse;
        public ConnectionService(string MyUrl)
        {
            myUrl = MyUrl;
        }
        public async Task HandleConnection(JObject json)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(myUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                string request = json.ToString();
                HttpContent content = new StringContent(request, Encoding.UTF8, "application/json");
                string buffer = await PostRequest(content, client);
                contextResponse = buffer;
                
            }
            
          
        }
        public async Task<string> PostRequest(HttpContent content, HttpClient client)
        {
            using (HttpResponseMessage resp = await client.PostAsync(myUrl, content))
            {
                if (resp.Content != null)
                {
                    string responseContent = await resp.Content.ReadAsStringAsync();
                    
                    return responseContent;
                }
                else
                {
                    string error = @"{
                            error: 'No response found'
                            }";
                    return error;
                }
            }
        }
        public RecaivedData GetDataFromString()
        {
            RecaivedData Data = new RecaivedData();

            return Data;
        }

    }
}
