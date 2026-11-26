using Domain.Entities.MusicEntities;
namespace Domain.Contracts;
public interface IArtistRepository : IGenericRepository<Artist, int>
{
	Task<bool> EmailExistsAsync(string email, int? excludingArtistId = null, CancellationToken cancellationToken = default);
	Task<bool> HasTracksAsync(int artistId, CancellationToken cancellationToken = default);
}
