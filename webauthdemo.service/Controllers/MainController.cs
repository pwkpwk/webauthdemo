using Microsoft.AspNetCore.Mvc;

namespace webauthdemo.service.Controllers;

[Route("api")]
public class MainController : Controller
{
    [HttpGet("GetRandomNumberAsync")]
    public async Task<ActionResult> GetRandomNumberAsync(CancellationToken cancellation)
    {
        await Task.Delay(Random.Shared.Next(10, 100), cancellation);
        return Ok(Random.Shared.Next());
    }
}