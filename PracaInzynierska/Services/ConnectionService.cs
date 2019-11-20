using System;
using System.Threading.Tasks;
using PracaInzynierska.Interfaces;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using PracaInzynierska.Models;
using System.IO;
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
                    contextResponse = responseContent;
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

            //string[] lines = contextResponse.Split('\n');
            string [] lines = File.ReadAllLines(@"C:\Users\USER\Desktop\WriteLines.txt");

            int i = 0;
            
            foreach (var line in lines)
            {
                if (i == 1500)
                {
                    Data.time = (double.Parse(line))/1000;
                    i++;
                }
                else if (i<1500)
                {
                    string[] measures = line.Split(',');
                    double m1 = double.Parse(measures[0]);
                    double m2 = double.Parse(measures[1]);
                    Data.bar[i] = (m1*6.1)/1000;
                    Data.hammer[i] = (m2*6.1)/1000;
                    i++;
                }
            }
            return Data;
        }

    }
}
