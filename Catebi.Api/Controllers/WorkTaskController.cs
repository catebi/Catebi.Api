using Catebi.Api.Data.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WorkTaskController : ControllerBase
{
    private readonly IWorkTaskService _workTaskService;

    public WorkTaskController(IWorkTaskService workTaskService)
    {
        _workTaskService = workTaskService;
    }

    [HttpGet]
    public async Task<List<WorkTopicDto>> GetTopics(string userTg) =>
        await _workTaskService.GetTopics(userTg);

    [HttpGet]
    public async Task<List<WorkTaskDto>> GetTasks(string userTg, int? topicId, bool onlyDone = false) =>
        await _workTaskService.GetTasks(userTg, topicId, onlyDone);

    [HttpGet]
    public async Task<List<WorkTaskDto>> GetVolunteerTasks(string userTg, int? topicId, bool onlyDone = false) =>
        await _workTaskService.GetVolunteerTasks(userTg, topicId, onlyDone);

    [HttpPost]
    public async Task<ActionResult<bool>> CreateTopic(CreateWorkTopicDto topic)
    {
        var res = await _workTaskService.CreateTopic(topic);
        if (res) return res;
        return NotFound("This volunteer not found");
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CreateTask(CreateWorkTaskDto task)
    {
        var res = await _workTaskService.CreateTask(task);
        if (res) return res;
        return NotFound();
    }

    [HttpPatch]
    public async Task<ActionResult<bool>> UpdateTask(int id, UpdateWorkTaskDto task)
    {
        var res = await _workTaskService.UpdateTask(id, task);
        if (res) return res;
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<bool>> RemoveTask(int id, string? userTg)
    {
        var res = await _workTaskService.RemoveTask(id, userTg);
        if (res) return res;
        return NotFound();
    }

    [HttpPatch]
    public async Task<ActionResult<bool>> ChangeTaskStatus(int id, WorkTaskStatuses newStatus, string? userTg)
    {
        var res = await _workTaskService.ChangeTaskStatus(id, newStatus, userTg);
        if (res) return res;
        return NotFound();
    }
}
