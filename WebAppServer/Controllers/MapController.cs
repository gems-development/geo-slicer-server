using DataAccess.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace WebAppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MapController : ControllerBase
{
    private readonly GeometryDbContext _context;
    private readonly Mediator _mediator;
    public MapController(GeometryDbContext context, Mediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    [HttpGet(Name = "Click")]
    [Route($"{{coordinate:{nameof(Coordinate)}}}")]
    public async Task<IActionResult> GetByClick([FromBody] Coordinate coordinate)
    {
        return Ok(await _mediator.Send(coordinate));
    }
    
    [HttpGet(Name = "Area")]
    public async Task<IActionResult> GetByArea()
    {
        return Ok();
    }
}