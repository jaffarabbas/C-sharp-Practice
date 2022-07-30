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
        private string apiUrl;
        private string controller;

        public ApiHandler(string apiUrl, string controller)
        {
            this.apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        private HttpClient httpClient;
        
        /// <summary>
        /// get all data from database api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetData<T>()
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.GetAsync(controller);
                response.Wait();
                var test = response.Result;
                if (!test.IsSuccessStatusCode) return null;
                var display = test.Content.ReadAsAsync<List<T>>();
                display.Wait();
                return display.Result;
            }
            catch(HttpRequestException error)
            {
                return (dynamic)error.Message.ToList();
            }
        }
        
        /// <summary>
        /// Get spesific data from database api with id
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>(int id)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.GetAsync(controller+"?id="+id.ToString());
                response.Wait();
                var test = response.Result;
                if (!test.IsSuccessStatusCode) return (dynamic)null;
                var display = test.Content.ReadAsAsync<T>();
                display.Wait();
                return display.Result;
            }
            catch(HttpRequestException error)
            {
                return (dynamic)error.Message.ToList();
            }
        }
        
        /// <summary>
        /// Insert Data to database api
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool PostData<T>(T obj)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.PostAsJsonAsync<T>(controller,obj);
                response.Wait();
                var test = response.Result;
                return test.IsSuccessStatusCode;
            }
            catch (HttpRequestException error)
            {
                return (dynamic)error.Message;
            }
        }

        /// <summary>
        /// Put data (update) data to database api
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool PutData<T>(T obj)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.PutAsJsonAsync<T>(controller, obj);
                response.Wait();
                var test = response.Result;
                return test.IsSuccessStatusCode;
            }
            catch (HttpRequestException error)
            {
                return (dynamic)error.Message;
            }
        }

        /// <summary>
        /// Delete spesific data from database api with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteData(int id)
        {
            try
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(apiUrl);
                var response = httpClient.DeleteAsync(controller + "?id=" + id.ToString());
                response.Wait();
                var test = response.Result;
                return test.IsSuccessStatusCode;
            }
            catch (HttpRequestException error)
            {
                return (dynamic)error.Message;
            }
        }
    }
}
