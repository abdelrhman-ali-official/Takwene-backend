using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared.DistributionModels;

namespace Services;

public class TrackDistributionService(IUnitOFWork unitOfWork) : ITrackDistributionService
{
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

    public async Task<IReadOnlyList<DistributionDTO>> DistributeAsync(int trackId, DistributeTrackRequestDTO request, CancellationToken cancellationToken = default)
    {
        ValidateId(trackId, "Track ID");

        if (request.DspIds is null || request.DspIds.Count == 0)
            throw new ValidationException(["At least one DSP ID is required."]);

        var dspIds = request.DspIds.ToArray();
        if (dspIds.Any(dspId => dspId <= 0))
            throw new ValidationException(["DSP IDs must be positive integers."]);

        var distinctDspIds = dspIds.Distinct().ToArray();
        if (distinctDspIds.Length != dspIds.Length)
            throw new ConflictException("Duplicate DSP IDs are not allowed in one request.");

        if (await unitOfWork.Tracks.GetAsync(trackId, cancellationToken) is null)
            throw new TrackNotFoundException(trackId);

        var missingDspIds = new List<int>();
        var dspNames = new Dictionary<int, string>();
        foreach (var dspId in distinctDspIds)
        {
            var dsp = await unitOfWork.DSPs.GetAsync(dspId, cancellationToken);
            if (dsp is null)
                missingDspIds.Add(dspId);
            else
                dspNames[dspId] = dsp.Name;
        }

        if (missingDspIds.Count > 0)
            throw new DspNotFoundException(missingDspIds[0]);

        foreach (var dspId in distinctDspIds)
        {
            if (await unitOfWork.TrackDistributions.ExistsAsync(trackId, dspId, cancellationToken))
                throw new ConflictException("This track is already distributed to one or more of the specified DSPs.");
        }

        var distributions = distinctDspIds.Select(dspId => new TrackDistribution
        {
            TrackId = trackId,
            DspId = dspId
        }).ToArray();

        foreach (var distribution in distributions)
            await unitOfWork.TrackDistributions.AddAsync(distribution, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return distributions
            .Select(distribution => MapToDTO(distribution, dspNames[distribution.DspId]))
            .ToArray();
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
