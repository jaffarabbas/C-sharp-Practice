using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace TestRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
        public List<ItemModel> itemModels { get; set; }
        public IndexModel(ILogger<IndexModel> logger,IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("TestApi");
        }

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7266/api/Test/GetAllItems");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                itemModels = JsonSerializer.Deserialize<List<ItemModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                itemModels = new List<ItemModel>();
            }
        }
    }
}
