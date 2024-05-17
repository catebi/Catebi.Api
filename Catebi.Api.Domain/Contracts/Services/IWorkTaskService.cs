namespace Catebi.Api.Domain.Contracts.Services;

public interface IWorkTaskService
{
    Task<List<WorkTopicDto>> GetTopics(string userTg);
    Task<List<WorkTaskDto>> GetTasks(string userTg, int? topicId, bool onlyDone = false);
    Task<List<WorkTaskDto>> GetVolunteerTasks(string userTg, int? topicId, bool onlyDone = false);
    Task<bool> CreateTask(WorkTaskDto task);
    Task<bool> UpdateTask(WorkTaskDto task);
    Task<bool> RemoveTask(int taskId);
    Task<bool> ChangeTaskStatus(int id, WorkTaskStatuses newStatus);
}
