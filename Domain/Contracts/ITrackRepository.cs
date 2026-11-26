using Domain.Entities.MusicEntities;
namespace Domain.Contracts;
public interface ITrackRepository : IGenericRepository<Track, int>
{
    Task<bool> IsIsrcUniqueAsync(string isrc, int? excludingTrackId = null, CancellationToken cancellationToken = default);
}
