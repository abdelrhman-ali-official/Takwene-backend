namespace Domain.Exceptions;

public sealed class ArtistHasTracksException()
    : Exception("An artist cannot be deleted while it has tracks.");