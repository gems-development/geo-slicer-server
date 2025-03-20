using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAppUseCases.Requests;
using WebAppUtils.Dto;

namespace WebAppServer.Controllers;

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
        // return Ok(coordinate.X + " " + coordinate.Y);
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate.CreatePoint()), token));
    }
    
    [HttpPost("byRectangle")]
    public async Task<IActionResult> GetByArea([FromBody]PointDto[] coordinates, CancellationToken token)
    {
        // return Ok(coordinates[0].X + " " + coordinates[0].Y + "|" + coordinates[1].X + " " + coordinates[1].Y);
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinates[0].CreatePoint(), coordinates[1].CreatePoint()), token));
    }
}