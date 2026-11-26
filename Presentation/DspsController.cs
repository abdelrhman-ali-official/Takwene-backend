using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DspModels;
using System.Net;

namespace Presentation;

public sealed class DspsController(IDSPService dspService) : ApiController
{
    [HttpGet("/api/dsps")]
    [ProducesResponseType(typeof(IReadOnlyList<DspDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<DspDTO>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await dspService.GetAllAsync(cancellationToken));

    [HttpGet("/api/dsps/{id:int}")]
    [ProducesResponseType(typeof(DspDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Shared.ErrorModels.ErrorDetails), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<DspDTO>> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await dspService.GetByIdAsync(id, cancellationToken));
}