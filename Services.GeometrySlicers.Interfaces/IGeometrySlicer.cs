using System.Collections.Generic;

namespace Services.GeometrySlicers.Interfaces
{
    public interface IGeometrySlicer<TGeometry, TSlicedType>
    {
        IEnumerable<TSlicedType> Slice(TGeometry polygon);
    }
}