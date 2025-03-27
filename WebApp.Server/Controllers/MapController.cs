using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp.UseCases.Requests;
using WebApp.Utils.Dto.Requests;

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
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate.CreatePoint()), token));
    }
    
    [HttpPost("byRectangle")]
    public async Task<IActionResult> GetByArea([FromBody][Length(2, 2)] PointDto[] coordinates, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinates[0].CreatePoint(), coordinates[1].CreatePoint()), token));
    }
}