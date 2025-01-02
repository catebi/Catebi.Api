using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ScheduleController(IDutyScheduleService scheduleService) : ControllerBase
{
    private readonly IDutyScheduleService _scheduleService = scheduleService;

    [HttpGet]
    public async Task<List<DutyScheduleUser>> GetAdminsCleaning() => await _scheduleService.GetAdminsCleaning();

    [HttpGet]
    public async Task<List<DutyScheduleUser>> GetVolunteersCleaning() => await _scheduleService.GetVolunteersCleaning();

}
