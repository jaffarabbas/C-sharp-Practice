using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApiFromScratch.Models;

namespace WebApiFromScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        public IActionResult GetAnimal()
        {
            var animal = new List<Animal>()
            {
                new Animal(){id = 1, name = "cat"},
            };
            return Ok (animal);
        }
    }
}
