namespace Domain.Contracts;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task VerifyInitializedAsync(CancellationToken cancellationToken = default);
}
