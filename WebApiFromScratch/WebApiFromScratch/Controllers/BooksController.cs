using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiFromScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        [Route("{id:int:min(10)}")]
        public string GetById(int id)
        {
            return "Id : " + id;
        }
        

        [Route("{id}")]
        public string GetByString(string id)
        {
            return "Id : " + id;
        }
    }
}
