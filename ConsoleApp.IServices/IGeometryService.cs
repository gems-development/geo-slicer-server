using System;

namespace ConsoleApp.IServices
{
    public interface IGeometryService<TGeometry, TKey>
    {
        TKey Save(TGeometry geometry);
    }
}