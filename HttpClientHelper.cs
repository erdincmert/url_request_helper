//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Url_Request_Helper
{
    public class HttpClientHelper
    {
        public class Root
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public List<Store> Result { get; set; }
        }
        public async Task<List<Store>> GetRequest(string url)
        {
            string response = null;

            using (HttpClient client = new HttpClient())
            {
                HttpClientHelper httpClientHelper = new HttpClientHelper();
                response = await httpClientHelper.RequestUrl(url, client);
            }

            Root root = JsonSerializer.Deserialize<Root>(response);

            if (root != null && root.Success)
            {
                return root.Result;
            }
            return null;
        }
        public async Task<string> RequestUrl(string URL, HttpClient client)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(URL);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }

            return null;
        }
    }
}
