namespace Catebi.Api.Domain.Contracts.Services;

public interface IFreeganMessageService
{
    Task<bool> SaveMessage(FreeganMessageDto message);
}
