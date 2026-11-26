using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.TrackModels;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Presentation;

public sealed class TracksController(ITrackService trackService) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(TrackDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<TrackDTO>> Create(TrackRequestDTO request, CancellationToken cancellationToken)
    {
        var track = await trackService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = track.Id }, track);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<TrackDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PaginatedResult<TrackDTO>>> GetAll(
        [FromQuery] int? artistId,
        [FromQuery] string? genre,
        [FromQuery] string? status,
        [FromQuery, Range(1, int.MaxValue)] int pageIndex = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        CancellationToken cancellationToken = default) =>
        Ok(await trackService.GetAllAsync(artistId, genre, status, pageIndex, pageSize, cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TrackDetailsDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<TrackDetailsDTO>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await trackService.GetByIdAsync(id, cancellationToken));

    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Update(int id, TrackRequestDTO request, CancellationToken cancellationToken)
    {
        await trackService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await trackService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}