namespace Domain.Exceptions;

public sealed class TrackNotFoundException(int trackId) : EntityNotFoundException($"Track with ID {trackId} was not found.");