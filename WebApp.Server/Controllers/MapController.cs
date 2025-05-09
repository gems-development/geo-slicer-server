using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dto.Requests;
using Mediatr.Queries;

namespace WebApp.Server.Controllers;

[ApiController]
[Route("geometry")]
public class MapController : ControllerBase
{
    private readonly IMediator _mediator;
    public MapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("byClick")]
    public async Task<IActionResult> GetByClick([FromBody] PointDto coordinate, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate), token));
    }
    
    [HttpPost("byRectangle")]
    public async Task<IActionResult> GetByArea([FromBody][Length(2, 2)] PointDto[] coordinates, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinates[0], coordinates[1]), token));
    }
}