using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class InboxController : ControllerBase
{
    [HttpPost("[action]")]
    public IActionResult Receive(object payload)
    {
        return Ok("This is a placeholder to represent how we would receive incoming messages such as product price and stock updates");
    }
}
