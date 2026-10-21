using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebPortal.Pages;

public class IndexModel(IHttpClientFactory httpClientFactory) : PageModel
{
    public Dictionary<string, string> Results { get; } = [];

    // Calls each service through the gateway to show the whole chain is wired up.
    public async Task OnGetAsync()
    {
        var gateway = httpClientFactory.CreateClient("gateway");

        foreach (var service in new[] { "employee", "payroll", "leave" })
        {
            try
            {
                var response = await gateway.GetAsync($"{service}/weatherforecast");
                Results[service] = $"{(int)response.StatusCode} {response.StatusCode}";
            }
            catch (HttpRequestException ex)
            {
                Results[service] = ex.Message;
            }
        }
    }
}
