
namespace Catebi.Api.Domain.Implementations.Services;

public class FreeganMessageService(  IUnitOfWork unitOfWork,
                                     ILogger<FreeganMessageService> logger
                                 ) : IFreeganMessageService
{
    private readonly ILogger<FreeganMessageService> _logger = logger;
    private readonly IFreeganMessageRepository _freeganRepo = unitOfWork.FreeganRepository;

    public async Task<bool> SaveMessage(FreeganMessageDto message)
    {
        try
        {
            var freeganMessage = new Message
            {
                OriginalText = message.OriginalText,
                LemmatizedText = message.LemmatizedText,
                ChatLink = message.ChatLink,
                Accepted = message.Accepted
            };

            await _freeganRepo.InsertAsync(freeganMessage);
            await unitOfWork.SaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving message");
            return false;
        }
    }

    #region Public



    #endregion

    #region Private

    #endregion

}
