namespace Catebi.Api.Domain.Implementations.Services;

public class DutyScheduleService(    IUnitOfWork unitOfWork,
                                     ILogger<DutyScheduleService> logger
                                 ) : IDutyScheduleService
{
    private readonly ILogger<DutyScheduleService> _logger = logger;
    private readonly IVolunteerRepository _volunteerRepo = unitOfWork.VolunteerRepository;

#region Public

    public async Task<List<DutyScheduleUser>> GetAdminsCleaning()
    {
        try
        {
            return await GetCleaningInternal(Roles.AdminCleaner);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting admin cleaners");
            throw;
        }
    }

    public async Task<List<DutyScheduleUser>> GetVolunteersCleaning()
    {
        try
        {
            return await GetCleaningInternal(Roles.VolunteerCleaner);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting volunteer cleaners");
            throw;
        }
    }

#endregion

#region Private

    private async Task<List<DutyScheduleUser>> GetCleaningInternal(Roles role)
    {
        var volunteers = await _volunteerRepo.GetAsync(
            filter: x => x.VolunteerRole.Any(y => y.RoleId == (int)role));

        return volunteers.Select(x => new DutyScheduleUser(x.Name, x.TelegramAccount!))
                         .ToList();
    }

#endregion

}
