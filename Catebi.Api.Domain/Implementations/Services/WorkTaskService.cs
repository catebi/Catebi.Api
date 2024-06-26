namespace Catebi.Api.Domain.Implementations.Services;

public class WorkTaskService(IUnitOfWork unitOfWork, ILogger<WorkTaskService> logger) : IWorkTaskService
{
    private readonly IVolunteerRepository _volunteerRepository = unitOfWork.VolunteerRepository;
    private readonly IWorkTaskRepository _workTaskRepository = unitOfWork.WorkTaskRepository;
    private readonly IWorkTopicRepository _workTopicRepository = unitOfWork.WorkTopicRepository;
    private readonly ILogger<WorkTaskService> _logger = logger;

    public async Task<List<WorkTopicDto>> GetTopics(string userTg)
    {
        var topics = await _workTopicRepository.GetAsync(filter: x => x.IsActual);

        var result = topics.Select(x => new WorkTopicDto
        {
            Id = x.WorkTopicId,
            TelegramThreadId = x.TelegramThreadId,
            Name = x.Name,
            Description = x.Description,
            IsMain = x.IsMain,
        }).ToList();

        return result;
    }

    public async Task<List<WorkTaskDto>> GetTasks(string userTg, int? topicId, bool onlyDone = false)
    {
        return await GetTasksInternal(userTg, topicId, onlyDone, forVolunteer: false);
    }

    public async Task<List<WorkTaskDto>> GetVolunteerTasks(string userTg, int? topicId, bool onlyDone = false)
    {
        return await GetTasksInternal(userTg, topicId, onlyDone, forVolunteer: true);
    }

    public Task<bool> CreateTask(WorkTaskDto task)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTask(WorkTaskDto task)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveTask(int taskId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangeTaskStatus(int id, WorkTaskStatuses newStatus)
    {
        throw new NotImplementedException();
    }

    #region Private

    private async Task<List<WorkTaskDto>> GetTasksInternal(string userTg, int? topicId, bool onlyDone = false, bool forVolunteer = false)
    {
        var vol = await _volunteerRepository.SingleAsync(x => x.TelegramAccount == userTg);
        var volId = forVolunteer ? vol.VolunteerId : (int?)null;

        var tasks = await _workTaskRepository.GetAsync(
            filter:
                x => (onlyDone
                        ? x.Status.Code == (int)WorkTaskStatuses.Done
                        : x.Status.Code == (int)WorkTaskStatuses.New || x.Status.Code == (int)WorkTaskStatuses.InProgress)
                    && ((topicId.HasValue && x.WorkTopicId == topicId) || !topicId.HasValue)
                    && (!volId.HasValue || x.WorkTaskResponsible.Any(y => y.VolunteerId == volId)),
            include: x => x.Include(y => y.Status)
                           .Include(y => y.CreatedBy)
                           .Include(y => y.ChangedBy)
                           .Include(y => y.WorkTopic)
                           .Include(y => y.WorkTaskResponsible)
                                .ThenInclude(y => y.Volunteer)
                           .Include(y => y.WorkTaskReminder));

        var result = tasks.Select(x => new WorkTaskDto
        {
            Id = x.WorkTaskId,
            Description = x.Description,
            TelegramThreadId = x.WorkTopic.TelegramThreadId,
            Status = x.Status.Name,
            StatusCode = x.Status.Code,
            ResponsibleUserTg = x.WorkTaskResponsible?.FirstOrDefault()?.Volunteer?.TelegramAccount,
            CreatedByTg = x.CreatedBy!.TelegramAccount!,
            CreatedDate = x.CreatedDate,
            ChangedByTg = x.ChangedBy!.TelegramAccount!,
            ChangedDate = x.ChangedDate,
            ReminderDate = x.WorkTaskReminder.Where(x => x.ReminderDate > DateTime.Now)
                                             .OrderBy(x => x.ReminderDate)
                                             .FirstOrDefault()?.ReminderDate,
        }).ToList();

        return result;
    }

    #endregion
}