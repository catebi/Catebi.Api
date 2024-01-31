using Catebi.Api.Data.Contracts.Repositories;

namespace Catebi.Api.Data.Contracts;

/// <summary>
/// Unit of work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    CatebiContext Context { get; }
    FreeganContext FreeganContext { get; }
    Task SaveAsync(int? userId = null);

    IFreeganMessageRepository FreeganRepository { get; }
    ICatRepository CatRepository { get; }
    IVolunteerRepository VolunteerRepository { get; }
}
