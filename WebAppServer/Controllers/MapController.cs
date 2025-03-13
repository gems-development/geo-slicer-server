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

    [HttpGet(Name = "Click")]
    [Route($"{{coordinate:{nameof(Point)}}}")]
    public async Task<IActionResult> GetByClick([FromBody] Point coordinate)
    {
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate)));
    }
    
    [HttpGet(Name = "Area")]
    [Route($"{{coordinateLeftBottom:{nameof(Point)}, coordinateRightTop:{nameof(Point)}}}")]
    public async Task<IActionResult> GetByArea([FromBody] Point coordinateLeftBottom, [FromBody] Point coordinateRightTop)
    {
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinateLeftBottom, coordinateRightTop)));
    }
}