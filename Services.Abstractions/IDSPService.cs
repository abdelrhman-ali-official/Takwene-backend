namespace Services.Abstractions;
public interface IDSPService { Task<bool> ExistsAsync(int dspId, CancellationToken cancellationToken = default); }
