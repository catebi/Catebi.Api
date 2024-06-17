namespace Catebi.Api.Domain.Contracts.Services;

public interface IWorkTaskService
{
    Task<List<WorkTopicDto>> GetTopics(string userTg);
    Task<List<WorkTaskDto>> GetTasks(string userTg, int? topicId, bool onlyDone = false);
    Task<List<WorkTaskDto>> GetVolunteerTasks(string userTg, int? topicId, bool onlyDone = false);
    Task<bool> CreateTopic(CreateWorkTopicDto topic);
    Task<bool> CreateTask(CreateWorkTaskDto task);
    Task<bool> UpdateTask(int id, UpdateWorkTaskDto task);
    Task<bool> RemoveTask(int id, string? userTg);
    Task<bool> ChangeTaskStatus(int id, WorkTaskStatuses newStatus, string? userTg);
}
