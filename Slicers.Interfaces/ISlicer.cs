using System.Collections.Generic;

namespace Slicers.Interfaces
{
    public interface ISlicer<TGeometry, TSlicedType>
    {
        IEnumerable<TSlicedType> Slice(TGeometry polygon);
    }
}