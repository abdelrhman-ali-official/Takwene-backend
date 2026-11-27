using Shared.DistributionModels;

namespace Services.Abstractions;

public interface ITrackDistributionService
{
	Task<IReadOnlyList<DistributionDTO>> GetForTrackAsync(int trackId, CancellationToken cancellationToken = default);
	Task<DistributionDTO> GetByIdAsync(int distributionId, CancellationToken cancellationToken = default);
	Task<bool> CanCreateAsync(int trackId, int dspId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<DistributionDTO>> DistributeAsync(int trackId, DistributeTrackRequestDTO request, CancellationToken cancellationToken = default);
}
