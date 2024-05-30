using Catebi.Api.Data.Models.Enums;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<bool> CreateTask(WorkTaskDto task) =>
        await _workTaskService.CreateTask(task);

    [HttpPost]
    public async Task<bool> RemoveTask(int id, string? userTg) =>
        await _workTaskService.RemoveTask(id, userTg);

    [HttpPost]
    public async Task<bool> ChangeTaskStatus(int id, WorkTaskStatuses newStatus, string? userTg) =>
        await _workTaskService.ChangeTaskStatus(id, newStatus, userTg);
}
