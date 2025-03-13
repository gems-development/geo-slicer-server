using DataAccess.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using WebAppUseCases.Requests;

namespace WebAppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MapController : ControllerBase
{
    private readonly Mediator _mediator;
    public MapController(Mediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Click")]
    // [Route($"{{coordinate:{nameof(Point)}}}")]
    public async Task<IActionResult> GetByClick([FromBody] Point coordinate, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate), token));
    }
    
    [HttpGet("Area")]
    // [Route($"{{coordinateLeftBottom:{nameof(Point)}, coordinateRightTop:{nameof(Point)}}}")]
    public async Task<IActionResult> GetByArea([FromBody](Point coordinateLeftBottom, Point coordinateRightTop) coordinates, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinates.coordinateLeftBottom, coordinates.coordinateRightTop), token));
    }
}