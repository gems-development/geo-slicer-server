using DataAccess.Repositories.ConsoleApp.Interfaces;
using DomainModels;
using NetTopologySuite.Geometries;

namespace DataAccess.Repositories.ConsoleApp
{
    public class MockRepository : IRepository<GeometryWithFragments<Polygon, Polygon>, int>
    {
        private int _i = 0;
        public int Save(GeometryWithFragments<Polygon, Polygon> polygon)
        {
            return _i++;
        }
    }
}