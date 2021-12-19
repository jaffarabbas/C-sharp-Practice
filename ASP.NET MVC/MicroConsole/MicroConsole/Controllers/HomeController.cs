using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MicroConsole.Controllers
{
    public class HomeController : ApiController
    {
        [Route("Home")]
        public string GetHome()
        {
            return "Here we go agin";
        }
    }
}

