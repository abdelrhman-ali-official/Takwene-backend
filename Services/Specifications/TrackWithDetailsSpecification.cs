using Domain.Contracts;
using Domain.Entities.MusicEntities;

namespace Services.Specifications;

public sealed class TrackWithDetailsSpecification : Specifications<Track>
{
    public TrackWithDetailsSpecification(int? artistId = null, string? genre = null, TrackStatus? status = null)
        : base(track => (!artistId.HasValue || track.ArtistId == artistId) &&
                        (string.IsNullOrWhiteSpace(genre) || track.Genre == genre) &&
                        (!status.HasValue || track.Status == status))
    {
        AddInclude(track => track.Artist);
        AddInclude("TrackDistributions.DSP");
    }
}
