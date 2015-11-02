using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SilverlightConfiguration
{
    public class ActServiceClient
    {
        private string _baseUrl;
        private HttpClientHandler _hander;
        public ActServiceClient(string baseAddress)
        {
            this._baseUrl = baseAddress;
            
        }

        public async Task<T> GetItemAsync<T>(string apiAddress) where T : class
        {
            T items = null;

            _hander = new HttpClientHandler();
            _hander.UseDefaultCredentials = true;

            using (var client = new HttpClient(_hander))
            {
                
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiAddress);
                if (response.IsSuccessStatusCode)
                {
                    items = await response.Content.ReadAsAsync<T>();
                }
            }
            return items;
        }

        public async Task CallService(string apiAddress)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiAddress);
            }
        }

        public async Task<bool> PostJsonItemAsync<T>(string apiAddress,T item)
        {
            bool isSuccess = false;
            _hander = new HttpClientHandler();
            _hander.UseDefaultCredentials = true;
            using(var client = new HttpClient(_hander))
            {
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<T>(apiAddress, item);

                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }

      
    }
}
