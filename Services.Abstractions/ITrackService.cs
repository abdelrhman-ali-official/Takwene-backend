namespace Services.Abstractions;
public interface ITrackService { Task<bool> IsIsrcAvailableAsync(string isrc, int? excludingTrackId = null, CancellationToken cancellationToken = default); }
