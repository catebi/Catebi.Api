using Microsoft.AspNetCore.Mvc;

namespace Catebi.Map.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ScheduleController : ControllerBase
{
    private readonly IDutyScheduleService _scheduleService;

    public ScheduleController(IDutyScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<List<DutyScheduleUser>> GetAdminsCleaning() => await _scheduleService.GetAdminsCleaning();

    [HttpGet]
    public async Task<List<DutyScheduleUser>> GetVolunteersCleaning() => await _scheduleService.GetVolunteersCleaning();

}
