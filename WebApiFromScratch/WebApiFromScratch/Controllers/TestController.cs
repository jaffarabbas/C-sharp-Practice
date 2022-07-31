using Microsoft.AspNetCore.Mvc;

namespace WebApiFromScratch.Controllers
{
    [ApiController]
    [Route("test/[action]")]
    public class TestController : ControllerBase
    {
        public string Get()
        {
            return "sadas";
        }
    }
}
