using Domain.Contracts;
using Services.Abstractions;
namespace Services;
public class ArtistService(IUnitOFWork unitOfWork) : IArtistService
{
    public async Task<bool> ExistsAsync(int artistId, CancellationToken cancellationToken = default) => await unitOfWork.Artists.GetAsync(artistId, cancellationToken) is not null;
}
