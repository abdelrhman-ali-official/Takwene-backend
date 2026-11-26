using Domain.Contracts;
using Services.Abstractions;
namespace Services;
public class DSPService(IUnitOFWork unitOfWork) : IDSPService
{
    public async Task<bool> ExistsAsync(int dspId, CancellationToken cancellationToken = default) => await unitOfWork.DSPs.GetAsync(dspId, cancellationToken) is not null;
}
