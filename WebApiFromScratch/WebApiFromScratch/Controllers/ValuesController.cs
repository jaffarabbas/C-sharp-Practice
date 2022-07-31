using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiFromScratch.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ValuesController : ControllerBase
    {
        //overriding base route
        [Route("~/api/get-employee")]
        //[Route("api/gte")]
        //[Route("api/[controller]/[action]")]
        public string GetEmployee()
        {
            return "Employee";
        }
        //[Route("api/get-ex-employee")]
        //[Route("api/gte")] this i not possible same route name are not possible runtime exception will occur
        //[Route("api/[controller]/[action]")]
        //e.g https://localhost:50584/api/values/getexemployee
        public string GetExEmployee()
        {
            return "Ex-Employee";
        }
        //variable in routing
        //[Route("api/get-employee/{id}")]
        [Route("{id}")]
        public string GetById(int id)
        {
            return "Id : " + id;
        }
        //[Route("api/get-employee/{id}/auther/{auther}")]
        [Route("{id}/auther/{auther}")]
        public string GetAutherById(int id,int auther)
        {
            return "Id : " + id + " auther : " + auther;
        }
        //[Route("Search")]
        public string GetValueByQueryString(int id, int auther,string name,int age)
        {
            return "hello";
        }
    }
}
