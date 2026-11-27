using Shared;
using Shared.TrackModels;

namespace Services.Abstractions;

public interface ITrackService
{
	Task<TrackDTO> CreateAsync(TrackRequestDTO request, CancellationToken cancellationToken = default);
	Task<PaginatedResult<TrackDTO>> GetAllAsync(int? artistId, string? genre, string? status, bool statusProvided, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
	Task<TrackDetailsDTO> GetByIdAsync(int trackId, CancellationToken cancellationToken = default);
	Task UpdateAsync(int trackId, TrackRequestDTO request, CancellationToken cancellationToken = default);
	Task UpdateStatusAsync(int trackId, int status, CancellationToken cancellationToken = default);
	Task DeleteAsync(int trackId, CancellationToken cancellationToken = default);
	Task<bool> IsIsrcAvailableAsync(string isrc, int? excludingTrackId = null, CancellationToken cancellationToken = default);
}
