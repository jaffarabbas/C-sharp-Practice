using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APIHandler
{
    public class ApiHandler
    {
        private HttpClient httpClient;
        public List<dynamic> GetData(string apiUrl,string controller)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.GetAsync(controller);
                response.Wait();
                var test = response.Result;
                if (test.IsSuccessStatusCode)
                {
                    var display = test.Content.ReadAsAsync<List<dynamic>>();
                    display.Wait();
                    return display.Result;
                }
                return null;
            }
            catch(HttpRequestException error)
            {
                return (dynamic)error.Message.ToList();
            }
        }
        
        public bool PostData<T>(string apiUrl, T obj,string controller)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.PostAsJsonAsync<T>(controller,obj);
                response.Wait();
                var test = response.Result;
                if (test.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (HttpRequestException error)
            {
                return (dynamic)error.Message;
            }
        }
    }
}
