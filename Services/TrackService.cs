using Domain.Contracts;
using Services.Abstractions;
namespace Services;
public class TrackService(IUnitOFWork unitOfWork) : ITrackService
{
    public Task<bool> IsIsrcAvailableAsync(string isrc, int? excludingTrackId = null, CancellationToken cancellationToken = default) => unitOfWork.Tracks.IsIsrcUniqueAsync(isrc, excludingTrackId, cancellationToken);
}
