using Domain.Contracts;
using Services.Abstractions;
namespace Services;
public class TrackDistributionService(IUnitOFWork unitOfWork) : ITrackDistributionService
{
    public async Task<bool> CanCreateAsync(int trackId, int dspId, CancellationToken cancellationToken = default) =>
        await unitOfWork.Tracks.GetAsync(trackId, cancellationToken) is not null &&
        await unitOfWork.DSPs.GetAsync(dspId, cancellationToken) is not null &&
        !await unitOfWork.TrackDistributions.ExistsAsync(trackId, dspId, cancellationToken);
}
