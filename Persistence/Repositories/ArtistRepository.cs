using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
namespace Persistence.Repositories;
public class ArtistRepository : GenericRepository<Artist, int>, IArtistRepository
{
	private readonly StoreContext _context;

	public ArtistRepository(StoreContext context) : base(context) => _context = context;

	public Task<bool> EmailExistsAsync(string email, int? excludingArtistId = null, CancellationToken cancellationToken = default) =>
		_context.Artists.AnyAsync(artist => artist.Email == email && (!excludingArtistId.HasValue || artist.Id != excludingArtistId.Value), cancellationToken);

	public Task<bool> HasTracksAsync(int artistId, CancellationToken cancellationToken = default) =>
		_context.Tracks.AnyAsync(track => track.ArtistId == artistId, cancellationToken);
}
