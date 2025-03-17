using DataAccess.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using WebAppUseCases.Requests;
using WebAppUtils;

namespace WebAppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MapController : ControllerBase
{
    private readonly IMediator _mediator;
    public MapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Click")]
    public async Task<IActionResult> GetByClick([FromBody] PointDecor coordinate, CancellationToken token)
    {
        return Ok(coordinate.X + " " + coordinate.Y);
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate), token));
    }
    
    [HttpPost("Area")]
    public async Task<IActionResult> GetByArea([FromBody]PointDecor[] coordinates, CancellationToken token)
    {
        return Ok(coordinates[0].X + " " + coordinates[0].Y + "|" + coordinates[1].X + " " + coordinates[1].Y);
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinates[0], coordinates[1]), token));
    }
}