using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ArtistModels;
using System.Net;

namespace Presentation;

public sealed class ArtistsController(IArtistService artistService) : ApiController
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost]
    [ProducesResponseType(typeof(ArtistDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult<ArtistDTO>> Create(ArtistRequestDTO request, CancellationToken cancellationToken)
    {
        var artist = await artistService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = artist.Id }, artist);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ArtistDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<ArtistDTO>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await artistService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArtistDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ArtistDTO>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await artistService.GetByIdAsync(id, cancellationToken));

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Update(int id, ArtistRequestDTO request, CancellationToken cancellationToken)
    {
        await artistService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await artistService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}