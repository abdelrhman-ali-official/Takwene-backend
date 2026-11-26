namespace Domain.Exceptions;

public sealed class DspNotFoundException(int dspId) : EntityNotFoundException($"DSP with ID {dspId} was not found.");