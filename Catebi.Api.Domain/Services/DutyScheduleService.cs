namespace Catebi.Api.Domain.Services;

public class DutyScheduleService : IDutyScheduleService
{
    private readonly CatebiContext _context;
    private readonly ILogger<DutyScheduleService> _logger;

    public DutyScheduleService(CatebiContext context, ILogger<DutyScheduleService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DutyScheduleUser>> GetAdminsCleaning()
    {
        var result = new List<DutyScheduleUser>();
        try
        {
            result = await GetAdminsCleaningInternal();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting cats");
            throw;
        }

        return result;
    }

    public async Task<List<DutyScheduleUser>> GetVolunteersCleaning()
    {
        var result = new List<DutyScheduleUser>();
        try
        {
            result = await GetVolunteersCleaningInternal();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting cats");
            throw;
        }

        return result;
    }

    #region Private

    private async Task<List<DutyScheduleUser>> GetAdminsCleaningInternal() =>
        await _context.VolunteerRole
            .Include(x => x.Volunteer)
            .Where(x => x.RoleId == (int)Roles.AdminCleaner)
            .Select(x => new DutyScheduleUser(x.Volunteer.Name, x.Volunteer.TelegramAccount!))
            .ToListAsync();

    private async Task<List<DutyScheduleUser>> GetVolunteersCleaningInternal() =>
        await _context.VolunteerRole
            .Include(x => x.Volunteer)
            .Where(x => x.RoleId == (int)Roles.VolunteerCleaner)
            .Select(x => new DutyScheduleUser(x.Volunteer.Name, x.Volunteer.TelegramAccount!))
            .ToListAsync();

    #endregion
}
