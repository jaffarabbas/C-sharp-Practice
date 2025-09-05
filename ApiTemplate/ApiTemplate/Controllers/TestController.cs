using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Shared; // Adjust namespace if needed

namespace ApiTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SkipJwtValidation]
    public class TestController : ControllerBase
    {
        // GET: api/test/query?msg=hello
        [HttpGet("query")]
        public IActionResult GetFromQuery([FromQuery] string msg)
        {
            return Ok(new { source = "query", message = msg ?? "No query message provided" });
        }

        // GET: api/test/direct/hello
        [HttpGet("direct/{msg}")]
        public IActionResult GetDirect(string msg)
        {
            return Ok(new { source = "direct", message = msg ?? "No direct message provided" });
        }

        // POST: api/test/body
        [HttpPost("body")]
        public IActionResult PostFromBody([FromBody] MessageDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { source = "body", message = "No body message provided" });

            return Ok(new { source = "body", message = dto.Message });
        }

        // PUT: api/test/put
        [HttpPut("put")]
        public IActionResult PutTest([FromBody] MessageDto dto)
        {
            return Ok(new { source = "put", message = dto?.Message ?? "No put message provided" });
        }

        // DELETE: api/test/delete?msg=bye
        [HttpDelete("delete")]
        public IActionResult DeleteTest([FromQuery] string msg)
        {
            return Ok(new { source = "delete", message = msg ?? "No delete message provided" });
        }
    }
}