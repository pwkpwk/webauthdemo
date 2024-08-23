using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webauthdemo.unggoy;

namespace webauthdemo.service.Controllers;

[Route("api")]
[Authorize("Unggoy")]
public class MainController : Controller
{
    [HttpGet("GetRandomNumberAsync")]
    [UnggoyAction("GetRandomNumberAsync")]
    public async Task<ActionResult> GetRandomNumberAsync(CancellationToken cancellation)
    {
        await Task.Delay(Random.Shared.Next(10, 100), cancellation);
        return Ok(Random.Shared.Next());
    }
}