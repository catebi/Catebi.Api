using Microsoft.AspNetCore.Mvc;

namespace Catebi.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class FreeganController : ControllerBase
{
    private readonly IFreeganService _freeganService;

    public FreeganController(IFreeganService freeganService)
    {
        _freeganService = freeganService;
    }

    [HttpPost]
    public async Task<bool> SaveMessage(FreeganMessageDto data) => await _freeganService.SaveMessage(data);

    [HttpGet]
    public async Task<List<DonationChatDto>> GetDonationChats() => await _freeganService.GetDonationChats();

    [HttpPost]
    public async Task<bool> SaveReaction(DonationMessageReactionDto data) => await _freeganService.SaveReaction(data);

    [HttpGet]
    public async Task<List<KeywordGroupDto>> GetSearchConfig() => await _freeganService.GetSearchConfig();
}
