using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;
using Shared;
using Shared.TrackModels;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Presentation;

public sealed class TracksController(ITrackService trackService) : ApiController
{
    [Authorize(AuthenticationSchemes = "Bearer")]
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
        CancellationToken cancellationToken = default)
    {
        var result = await trackService.GetAllAsync(
            artistId,
            genre,
            status,
            Request.Query.ContainsKey("status"),
            pageIndex,
            pageSize,
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TrackDetailsDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<TrackDetailsDTO>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await trackService.GetByIdAsync(id, cancellationToken));

    [Authorize(AuthenticationSchemes = "Bearer")]
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

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, TrackStatusRequestDTO request, CancellationToken cancellationToken)
    {
        await trackService.UpdateStatusAsync(id, request.Status, cancellationToken);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
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