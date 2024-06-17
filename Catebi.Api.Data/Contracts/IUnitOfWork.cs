namespace Catebi.Api.Data.Contracts;

/// <summary>
/// Unit of work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    CatebiContext Context { get; }
    Task SaveAsync(int? userId = null);

    IFreeganMessageRepository FreeganRepository { get; }
    IWorkTaskRepository WorkTaskRepository { get; }
    IWorkTaskResponsibleRepository WorkTaskResponsibleRepository { get; }
    IWorkTopicRepository WorkTopicRepository { get; }
    
    ICatRepository CatRepository { get; }
    IVolunteerRepository VolunteerRepository { get; }
    IDonationChatRepository DonationChatRepository { get; }
    IDonationMessageReactionRepository DonationMessageReactionRepository { get; }
    IKeywordGroupRepository KeywordGroupRepository { get; }
}
