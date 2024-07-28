using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[Route("api/[controller]/[action]")]
public class TestAuthController : ControllerBase
{
    private readonly ILogger<TestAuthController> _logger;

    public TestAuthController(ILogger<TestAuthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetWeather()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                //Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
            .ToArray();

        return Ok(forecast);
    }

}
