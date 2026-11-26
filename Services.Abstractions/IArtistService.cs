namespace Services.Abstractions;
public interface IArtistService { Task<bool> ExistsAsync(int artistId, CancellationToken cancellationToken = default); }
