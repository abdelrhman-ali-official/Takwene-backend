using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.DistributionModels;

namespace Services;

public class TrackDistributionService(IUnitOFWork unitOfWork) : ITrackDistributionService
{
    public async Task<DistributionDTO> CreateAsync(int trackId, DistributionRequestDTO request, CancellationToken cancellationToken = default)
    {
        ValidateId(trackId, "Track ID");
        ValidateId(request.DspId, "DSP ID");

        if (await unitOfWork.Tracks.GetAsync(trackId, cancellationToken) is null)
            throw new TrackNotFoundException(trackId);
        if (await unitOfWork.DSPs.GetAsync(request.DspId, cancellationToken) is null)
            throw new DspNotFoundException(request.DspId);
        if (await unitOfWork.TrackDistributions.ExistsAsync(trackId, request.DspId, cancellationToken))
            throw new ConflictException("This track is already distributed to the specified DSP.");

        var distribution = new TrackDistribution
        {
            TrackId = trackId,
            DspId = request.DspId
        };
        await unitOfWork.TrackDistributions.AddAsync(distribution, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dsp = await unitOfWork.DSPs.GetAsync(request.DspId, cancellationToken)
            ?? throw new DspNotFoundException(request.DspId);
        return MapToDTO(distribution, dsp.Name);
    }

    public async Task<IReadOnlyList<DistributionDTO>> GetForTrackAsync(int trackId, CancellationToken cancellationToken = default)
    {
        ValidateId(trackId, "Track ID");
        if (await unitOfWork.Tracks.GetAsync(trackId, cancellationToken) is null)
            throw new TrackNotFoundException(trackId);

        var distributions = await unitOfWork.TrackDistributions.GetAllAsync(
            new DistributionsForTrackSpecification(trackId), cancellationToken);
        return distributions.Select(distribution => MapToDTO(distribution, distribution.DSP.Name)).ToArray();
    }

    public async Task<DistributionDTO> GetByIdAsync(int distributionId, CancellationToken cancellationToken = default)
    {
        ValidateId(distributionId, "Distribution ID");
        var distribution = await unitOfWork.TrackDistributions.GetAsync(
            new DistributionByIdSpecification(distributionId), cancellationToken)
            ?? throw new TrackDistributionNotFoundException(distributionId);
        return MapToDTO(distribution, distribution.DSP.Name);
    }

    public async Task<bool> CanCreateAsync(int trackId, int dspId, CancellationToken cancellationToken = default)
    {
        if (trackId <= 0 || dspId <= 0)
            return false;

        return await unitOfWork.Tracks.GetAsync(trackId, cancellationToken) is not null &&
               await unitOfWork.DSPs.GetAsync(dspId, cancellationToken) is not null &&
               !await unitOfWork.TrackDistributions.ExistsAsync(trackId, dspId, cancellationToken);
    }

    private static void ValidateId(int id, string name)
    {
        if (id <= 0)
            throw new ValidationException([$"{name} must be a positive integer."]);
    }

    private static DistributionDTO MapToDTO(TrackDistribution distribution, string dspName) => new(
        distribution.Id,
        distribution.TrackId,
        distribution.DspId,
        dspName,
        distribution.SubmittedAt,
        distribution.Status.ToString());
}
