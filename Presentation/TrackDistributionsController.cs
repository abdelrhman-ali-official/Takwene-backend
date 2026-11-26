using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DistributionModels;
using System.Net;

namespace Presentation;

public sealed class TrackDistributionsController(ITrackDistributionService distributionService) : ApiController
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost("/api/tracks/{trackId:int}/distributions")]
    [ProducesResponseType(typeof(DistributionDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<DistributionDTO>> Create(
        int trackId,
        DistributionRequestDTO request,
        CancellationToken cancellationToken)
    {
        var distribution = await distributionService.CreateAsync(trackId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = distribution.Id }, distribution);
    }

    [HttpGet("/api/tracks/{trackId:int}/distributions")]
    [ProducesResponseType(typeof(IReadOnlyList<DistributionDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IReadOnlyList<DistributionDTO>>> GetForTrack(int trackId, CancellationToken cancellationToken) =>
        Ok(await distributionService.GetForTrackAsync(trackId, cancellationToken));

    [HttpGet("/api/distributions/{id:int}")]
    [ProducesResponseType(typeof(DistributionDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<DistributionDTO>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await distributionService.GetByIdAsync(id, cancellationToken));
}