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
    private readonly Mediator _mediator;
    public MapController(Mediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Click")]
    public async Task<IActionResult> GetByClick([FromBody] PointDecor coordinate, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByClickQuery(coordinate), token));
    }
    
    [HttpGet("Area")]
    public async Task<IActionResult> GetByArea([FromBody](PointDecor coordinateLeftBottom, PointDecor coordinateRightTop) coordinates, CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetByAreaQuery(coordinates.coordinateLeftBottom, coordinates.coordinateRightTop), token));
    }
}