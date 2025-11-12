// V1 Controller
using Microsoft.AspNetCore.Mvc;
using ApiTemplate.Shared.Dtos; // Adjust namespace if needed
using ApiTemplate.Shared.Helper.Constants;
using Asp.Versioning;
using ApiTemplate.Attributes;

namespace ApiTemplate.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    [SkipPermissionCheck]
    [SkipJwtValidation]
    public class TestController : ControllerBase
    {
        // GET: api/v1/test/query?msg=hello
        [HttpGet("query")]
        public IActionResult GetFromQuery([FromQuery] string msg)
        {
            return Ok(new { source = "query v1", message = msg ?? "No query message provided", version = "1.0" });
        }

        // GET: api/v1/test/direct/hello
        [HttpGet("direct/{msg}")]
        public IActionResult GetDirect(string msg)
        {
            return Ok(new { source = "direct v1", message = msg ?? "No direct message provided", version = "1.0" });
        }

        // POST: api/v1/test/body
        [HttpPost("body")]
        public IActionResult PostFromBody([FromBody] TestMessage dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { source = "body v1", message = "No body message provided" });

            return Ok(new { source = "body v1", message = dto.Message, version = "1.0" });
        }

        // PUT: api/v1/test/put
        [HttpPut("put")]
        public IActionResult PutTest([FromBody] TestMessage dto)
        {
            return Ok(new { source = "put v1", message = dto?.Message ?? "No put message provided", version = "1.0" });
        }

        // DELETE: api/v1/test/delete?msg=bye
        [HttpDelete("delete")]
        public IActionResult DeleteTest([FromQuery] string msg)
        {
            return Ok(new { source = "delete v1", message = msg ?? "No delete message provided", version = "1.0" });
        }
    }
}

// V2 Controller - Same name but different namespace
namespace ApiTemplate.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiExplorerSettings(GroupName = "v2")]
    [SkipJwtValidation]
    public class TestController : ControllerBase  // Same name as V1
    {
        // GET: api/v2/test/query?msg=hello
        [HttpGet("query")]
        public IActionResult GetFromQuery([FromQuery] string msg)
        {
            return Ok(new { source = "query v2", message = $"v2: {msg ?? "No query message provided"}", version = "2.0" });
        }

        // GET: api/v2/test/direct/hello
        [HttpGet("direct/{msg}")]
        public IActionResult GetDirect(string msg)
        {
            return Ok(new { source = "direct v2", message = $"v2: {msg ?? "No direct message provided"}", version = "2.0" });
        }

        // POST: api/v2/test/body
        [HttpPost("body")]
        public IActionResult PostFromBody([FromBody] TestMessage dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { source = "body v2", message = "No body message provided" });

            return Ok(new { source = "body v2", message = $"v2: {dto.Message}", version = "2.0" });
        }

        // New endpoint only available in V2
        [HttpGet("newfeature")]
        public IActionResult GetNewFeature()
        {
            return Ok(new { source = "new feature", message = "This endpoint is only available in v2", version = "2.0" });
        }
    }
}