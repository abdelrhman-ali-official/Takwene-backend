namespace Domain.Exceptions;

public sealed class ArtistNotFoundException(int artistId) : EntityNotFoundException($"Artist with ID {artistId} was not found.");