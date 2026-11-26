namespace Domain.Exceptions;

public sealed class TrackDistributionNotFoundException(int distributionId)
    : EntityNotFoundException($"Distribution with ID {distributionId} was not found.");