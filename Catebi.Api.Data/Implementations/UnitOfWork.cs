namespace Catebi.Api.Data.Implementations;

/// <summary>
/// UnitOfWork
/// </summary>
public class UnitOfWork (   CatebiContext context,
                            //IClaimsProvider claimsProvider,
                            ICatRepository catRepo,
                            IVolunteerRepository volunteerRepo,
                            IFreeganMessageRepository freeganRepo,
                            IWorkTaskRepository workTaskRepo,
                            IWorkTopicRepository workTopicRepo,
                            IDonationChatRepository chatRepo,
                            IDonationMessageReactionRepository reactionRepo,
                            IKeywordGroupRepository keywordGroupRepo
                        ) : IUnitOfWork
{
    #region Cstor

    private bool disposed = false;
    public CatebiContext Context { get; } = context;

    //private readonly IClaimsProvider ClaimsProvider;

    public ICatRepository CatRepository { get; private set; } = catRepo;
    public IFreeganMessageRepository FreeganRepository { get; private set; } = freeganRepo;
    public IWorkTaskRepository WorkTaskRepository { get; private set; } = workTaskRepo;
    public IWorkTopicRepository WorkTopicRepository { get; private set; } = workTopicRepo;
    public IVolunteerRepository VolunteerRepository { get; private set; } = volunteerRepo;
    public IDonationChatRepository DonationChatRepository { get; private set; } = chatRepo;
    public IDonationMessageReactionRepository DonationMessageReactionRepository { get; private set; } = reactionRepo;
    public IKeywordGroupRepository KeywordGroupRepository { get; private set; } = keywordGroupRepo;

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task SaveAsync(int? userId = null)
    {
        //AuditEntities(userId);
        await Context.SaveChangesAsync();
    }

/*
    private void AuditEntities(int? userIden = null)
    {
        Context.ChangeTracker.DetectChanges();
        var entities = Context.ChangeTracker.Entries()
            .Where(t => t.Entity is IAuditEntity && (t.State == EntityState.Added || t.State == EntityState.Modified));

        if (entities.Any())
        {
            var timestamp = DateTime.UtcNow;
            var userId = userIden ?? ClaimsProvider.UserId;

            foreach (var entry in entities)
            {
                var entity = (IAuditEntity)entry.Entity;

                entity.ChangeDate = timestamp;

                if (userId != 0)
                {
                    entity.ChangedById = userId;
                }

                if (entry.State == EntityState.Added)
                {
                    entity.CreateDate = timestamp;
                    entity.CreatedById = userId;
                }
            }
        }
    }
    */
}
