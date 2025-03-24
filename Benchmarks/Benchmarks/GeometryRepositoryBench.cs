using BenchmarkDotNet.Attributes;
using DataAccess.PostgreSql;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using WebApp.UseCases.Repositories;
using WebApp.UseCases.Repositories.Interfaces;

namespace Benchmarks.Benchmarks;

public class GeometryRepositoryBench
{
    private static readonly Polygon ScreenSmall = new Polygon(new LinearRing(new Coordinate[]
    {
        new(106.92672, 53.48174), new(107.74302, 53.47658), new(107.74007, 52.87246), new(106.92745, 52.86951),
        new(106.92672, 53.48174)
    }));

    private static readonly Polygon ScreenFull = new Polygon(new LinearRing(new Coordinate[]
        { new(103, 57), new(111, 57), new(111, 51), new(103, 51), new(103, 57) }));

    private static readonly Polygon ScreenBig = new Polygon(new LinearRing(new Coordinate[]
    {
        new(104.3503, 51.6423), new(104.6685, 55.0666), new(109.4779, 55.2317), new(109.3306, 51.4360),
        new(104.3503, 51.6423)
    }));

    private static readonly PostgreApplicationContext PgContext =
        new("Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin");
    
    private static IGeometryRepository<Geometry> _repository = new GeometryRepository(PgContext);

    [Benchmark]
    public async Task<Geometry> GetGeometryByPolygonEnumerateFragmentsOnSmallScreen()
    {
        return await _repository.GetGeometryByPolygonEnumerateFragments(ScreenSmall);
    }
    
    [Benchmark]
    public async Task<Geometry> GetGeometryByPolygonLinqOnSmallScreen()
    {
        return await _repository.GetGeometryByPolygonLinq(ScreenSmall);
    }
    
    [Benchmark]
    public async Task<Geometry> GetGeometryByPolygonEnumerateFragmentsOnBigScreen()
    {
        return await _repository.GetGeometryByPolygonEnumerateFragments(ScreenBig);
    }
    
    [Benchmark]
    public async Task<Geometry> GetGeometryByPolygonLinqOnBigScreen()
    {
        return await _repository.GetGeometryByPolygonLinq(ScreenBig);
    }
    
    [Benchmark]
    public async Task<Geometry> GetGeometryByPolygonEnumerateFragmentsOnFullScreen()
    {
        return await _repository.GetGeometryByPolygonEnumerateFragments(ScreenFull);
    }
    
    [Benchmark]
    public async Task<Geometry> GetGeometryByPolygonLinqOnFullScreen()
    {
        return await _repository.GetGeometryByPolygonLinq(ScreenFull);
    }
}