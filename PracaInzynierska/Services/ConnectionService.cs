using System;
using System.Threading.Tasks;
using PracaInzynierska.Interfaces;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;

namespace PracaInzynierska.Services
{

    public class ConnectionService : IConnectionService

    {
        public string myUrl;
        static public string contextResponse;
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

            }

            return;
        }
        public async Task<string> PostRequest(HttpContent content, HttpClient client)
        {
            using (HttpResponseMessage resp = await client.PostAsync(myUrl, content))
            {
                if (resp.Content != null)
                {
                    string responseContent = await resp.Content.ReadAsStringAsync();
                    int index = responseContent.IndexOf("}");
                    if (index > 0)
                        responseContent = responseContent.Substring(0, index + 1);
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
    }
}
