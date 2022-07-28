using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPIWithSQL.Models;

namespace WebAPIWithSQL.Controllers.Api
{
    public class StudentApiController : ApiController
    {
        DB_TestEntities dB_Test = new DB_TestEntities();

        [HttpGet]
        public IHttpActionResult Index()
        {
            List<STUDENT> data = dB_Test.STUDENTS.ToList(); 
            return Ok(data);
        }

        [HttpGet]
        public IHttpActionResult Index(int id)
        {
            var data = dB_Test.STUDENTS.Where(d => d.ID == id).FirstOrDefault();
            return Ok(data);
        }
    }
}
