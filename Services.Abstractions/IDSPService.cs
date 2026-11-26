using Shared.DspModels;

namespace Services.Abstractions;

public interface IDSPService
{
	Task<IReadOnlyList<DspDTO>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<DspDTO> GetByIdAsync(int dspId, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(int dspId, CancellationToken cancellationToken = default);
}
