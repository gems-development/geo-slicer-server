using ConsoleApp.IRepositories;
using DomainModels;
using NetTopologySuite.Geometries;

namespace ConsoleApp.Repositories
{
    public class MockSaveRepository : ISaveRepository<GeometryWithFragments<Polygon, Polygon>, int>
    {
        private int _i = 0;
        public int Save(GeometryWithFragments<Polygon, Polygon> polygon)
        {
            return _i++;
        }
    }
}