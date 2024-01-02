namespace Catebi.Api.Domain.Contracts.Services;

public interface IDutyScheduleService
{
    Task<List<DutyScheduleUser>> GetAdminsCleaning();
    Task<List<DutyScheduleUser>> GetVolunteersCleaning();
}