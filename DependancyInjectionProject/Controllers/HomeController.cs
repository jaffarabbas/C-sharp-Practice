using DependancyInjectionProject.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DependancyInjectionProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IEmailRepo emailRepo;
    private readonly IServiceProvider serviceProvider;
    public HomeController(IEmailRepo emailRepo, IServiceProvider serviceProvider)
    {
        this.emailRepo = emailRepo;
        this.serviceProvider = serviceProvider;
    }
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> Index()
    {
        var res = await emailRepo.SendEmailAsync("", "", "");
        return Ok(res);
    }

    [HttpGet]
    [Route("Serviceprovider")]
    public async Task<IActionResult> Index2()
    {
        var reo = serviceProvider.GetService<IEmailRepo>();
        var res = await reo!.SendEmailAsync("", "", "");
        return Ok(res);
    }
}