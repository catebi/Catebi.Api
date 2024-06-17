using System.Diagnostics;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Catebi.Api.Data.Implementations.Repositories;

namespace Catebi.Api.Domain.Implementations.Services;

public class WorkTaskService(IUnitOfWork unitOfWork, ILogger<WorkTaskService> logger) : IWorkTaskService
{
    private readonly IVolunteerRepository _volunteerRepository = unitOfWork.VolunteerRepository;
    private readonly IWorkTaskRepository _workTaskRepository = unitOfWork.WorkTaskRepository;
    private readonly IWorkTaskResponsibleRepository _workTaskResponsibleRepository = unitOfWork.WorkTaskResponsibleRepository;
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

    public async Task<bool> CreateTopic(CreateWorkTopicDto topic)
    {
        try
        {
            var volId = await GetVolunteerId(topic.CreatedBy);

            if (volId == 0) return false;

            var newTopic = new WorkTopic
            {
                TelegramThreadId = topic.TelegramThreadId,
                Name = topic.Name,
                Description = topic.Description,
                Created = DateTime.Now,
                CreatedById = volId
            };
            await _workTopicRepository.InsertAsync(newTopic);
            await unitOfWork.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating topic");
            return false;
        }
    }

    public async Task<bool> CreateTask(CreateWorkTaskDto task)
    {
        if (task.TgThread == 0) throw new ArgumentException("TgThread is required.");

        try
        {
            var updatedMessage = ParseTaskMessage(task.Message);
            var responsibleUserTg = updatedMessage[0];
            var taskText = updatedMessage[1];

            var volId = await GetVolunteerId(task.UserTg);
            var responsibleVolId = await GetVolunteerId(responsibleUserTg);
            var topicId = await GetTopicByTgThread(task.TgThread);

            if (volId == 0 || responsibleVolId == 0 || topicId == 0) return false;

            var newTask = new WorkTask
            {
                Description = taskText,
                StatusId = (int)WorkTaskStatuses.New,
                CreatedDate = DateTime.Now,
                CreatedById = volId,
                ChangedById = volId,
                WorkTopicId = topicId
            };

            var createdTask = await _workTaskRepository.InsertAsync(newTask);
            await unitOfWork.SaveAsync();

            var newResponsibleUser = new WorkTaskResponsible
            {
                WorkTaskId = createdTask.WorkTaskId,
                VolunteerId = responsibleVolId
            };
            await _workTaskResponsibleRepository.InsertAsync(newResponsibleUser);
            await unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            throw;
        }
    }

    public async Task<bool> UpdateTask(int id, UpdateWorkTaskDto task)
    {
        try
        {
            var workTask = await _workTaskRepository.GetByIdAsync(id);
            if (workTask == null) return false;

            var updatedMessage = ParseTaskMessage(task.Message);
            var responsibleUserTg = updatedMessage[0];
            var taskText = updatedMessage[1];

            var volId = await GetVolunteerId(task.UserTg);
            var previousResponsibleUser = await _workTaskResponsibleRepository.SingleOrDefaultAsync(filter: x => x.WorkTaskId == workTask.WorkTaskId);
            var responsibleUserId = await GetVolunteerId(responsibleUserTg);

            if (volId == 0 || previousResponsibleUser == null || responsibleUserId == 0) return false;

            if (previousResponsibleUser.VolunteerId != responsibleUserId)
            {
                previousResponsibleUser.VolunteerId = responsibleUserId;
                _workTaskResponsibleRepository.Update(previousResponsibleUser);
                await unitOfWork.SaveAsync();
            }

            workTask.Description = taskText;
            workTask.ChangedDate = DateTime.Now;
            workTask.ChangedById = volId;

            _workTaskRepository.Update(workTask);            
            await unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task");
            throw;
        }
    }

    public async Task<bool> RemoveTask(int id, string? userTg)
    {
        var volId = !string.IsNullOrEmpty(userTg) ? await GetVolunteerId(userTg) : (int?)null;
        if (volId == 0) return false;
        return await ChangeStatus(id, WorkTaskStatuses.Deleted, volId);
    }

    public async Task<bool> ChangeTaskStatus(int id, WorkTaskStatuses newStatus, string? userTg)
    {
        var volId = !string.IsNullOrEmpty(userTg) ? await GetVolunteerId(userTg) : (int?)null;
        if (volId == 0) return false;
        return await ChangeStatus(id, newStatus, volId);
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

    private async Task<bool> ChangeStatus(int id, WorkTaskStatuses newStatus, int? volId = null)
    {
        try
        {
            var task = await _workTaskRepository.GetByIdAsync(id);

            if (task == null) return false;

            if (volId.HasValue) task.ChangedById = volId.Value;

            task.ChangedDate = DateTime.Now;
            task.StatusId = (int)newStatus;
            _workTaskRepository.Update(task);

            await unitOfWork.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing task status");
            return false;
        }
    }

    private async Task<int> GetVolunteerId(string userTg)
    {
        var vol = await _volunteerRepository.SingleOrDefaultAsync(filter: x => x.TelegramAccount.Equals(userTg));
        if (vol == null) return 0;
        return vol.VolunteerId;
    }

    private async Task<int> GetTopicByTgThread(int thread)
    {
        var topic = await _workTopicRepository.SingleOrDefaultAsync(filter: x => x.TelegramThreadId.Equals(thread));
        if (topic == null) return 0;
        return topic.WorkTopicId;
    }

    private static string[] ParseTaskMessage(string message)
    {
        var extractedWord = "";
        var pattern = @"@\w+(?!\w\.)";

        var match = Regex.Match(message, pattern);

        if (match.Success)
        {
            extractedWord = match.Value;
        }
        var remainingText = Regex.Replace(message, pattern, "");

        return [extractedWord, remainingText];
    }

    #endregion
}