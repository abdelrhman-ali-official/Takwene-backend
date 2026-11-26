using Domain.Contracts;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.DspModels;

namespace Services;

public class DSPService(IUnitOFWork unitOfWork) : IDSPService
{
    public async Task<IReadOnlyList<DspDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dsps = await unitOfWork.DSPs.GetAllAsync(cancellationToken);
        return dsps.Select(dsp => new DspDTO(dsp.Id, dsp.Name)).ToArray();
    }

    public async Task<DspDTO> GetByIdAsync(int dspId, CancellationToken cancellationToken = default)
    {
        if (dspId <= 0)
            throw new ValidationException(["DSP ID must be a positive integer."]);

        var dsp = await unitOfWork.DSPs.GetAsync(dspId, cancellationToken)
            ?? throw new DspNotFoundException(dspId);
        return new DspDTO(dsp.Id, dsp.Name);
    }

    public async Task<bool> ExistsAsync(int dspId, CancellationToken cancellationToken = default) => await unitOfWork.DSPs.GetAsync(dspId, cancellationToken) is not null;
}
