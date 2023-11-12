namespace Catebi.Api.Domain.Interfaces;

public interface IDutyScheduleService
{
    Task<List<DutyScheduleUser>> GetAdminsCleaning();
    Task<List<DutyScheduleUser>> GetVolunteersCleaning();
}