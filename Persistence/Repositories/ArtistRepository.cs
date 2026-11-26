using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Persistence.Data;
namespace Persistence.Repositories;
public class ArtistRepository(StoreContext context) : GenericRepository<Artist, int>(context), IArtistRepository
{
}
