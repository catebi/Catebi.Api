using Catebi.Api.Data.Contracts.Repositories;
using Catebi.Api.Data.Implementations.Repositories;

namespace Catebi.Api.Data.Implementations.Repositories;

public class VolunteerRepository(CatebiContext context) : BaseRepository<Volunteer>(context), IVolunteerRepository
{
}
