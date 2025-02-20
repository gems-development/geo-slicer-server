using DataAccess.PostgreSql;
using DataAccess.Repositories.ConsoleApp;
using DataAccess.Repositories.GeometryUnion;
using DomainModels;
using NetTopologySuite.Geometries;
using Xunit.Abstractions;

namespace GeometryUnifierTests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test()
    {
        String connectionString = "Host=localhost;Port=5432;Database=demo;Username=postgres;Password=admin";
        var pgContext = new PostgreApplicationContext(connectionString);
        var polygons = pgContext.GeometryFragments.Where(o => o.GeometryOriginalId == 18).Select(o => o.Fragment).ToList();
        var result = GeometryUnifier.Union(polygons);
        new SaveRepository(pgContext).Save(new GeometryWithFragments<Polygon,FragmentWithNonRenderingBorder<Polygon,MultiLineString>>(result, new List<FragmentWithNonRenderingBorder<Polygon,MultiLineString>>()));
    }
}