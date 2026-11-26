using Shared.ArtistModels;

namespace Services.Abstractions;
public interface IArtistService
{
	Task<ArtistDTO> CreateAsync(ArtistRequestDTO request, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<ArtistDTO>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<ArtistDTO> GetByIdAsync(int artistId, CancellationToken cancellationToken = default);
	Task UpdateAsync(int artistId, ArtistRequestDTO request, CancellationToken cancellationToken = default);
	Task DeleteAsync(int artistId, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(int artistId, CancellationToken cancellationToken = default);
}
