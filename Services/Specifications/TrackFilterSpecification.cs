using Domain.Contracts;
using Domain.Entities.MusicEntities;

namespace Services.Specifications;

public sealed class TrackFilterSpecification : Specifications<Track>
{
    public TrackFilterSpecification(int? artistId, string? genre, TrackStatus? status)
        : base(track => (!artistId.HasValue || track.ArtistId == artistId.Value) &&
                        (string.IsNullOrWhiteSpace(genre) || track.Genre == genre) &&
                        (!status.HasValue || track.Status == status.Value))
    {
        setOrderBy(track => track.Id);
    }
}