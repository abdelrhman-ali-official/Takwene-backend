namespace Services.Abstractions;
public interface ITrackDistributionService { Task<bool> CanCreateAsync(int trackId, int dspId, CancellationToken cancellationToken = default); }
