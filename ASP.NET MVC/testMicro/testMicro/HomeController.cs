using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace testMicro
{
    //[EnableCors("*", "*", "*")]
    public class HomeController : ApiController
    {
        [Route("Home")]
        public IHttpActionResult GetHome()
        {
            return Ok("Hiii jaffar");
        }
    }
}
