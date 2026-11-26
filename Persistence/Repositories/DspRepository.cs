using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Persistence.Data;
namespace Persistence.Repositories;
public class DspRepository(StoreContext context) : GenericRepository<DSP, int>(context), IDspRepository
{
}
