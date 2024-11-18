using System.Collections.Generic;

namespace Slicers.Interfaces
{
    public interface ISlicer<TK, TV>
    {
        IEnumerable<TV> Slice(TK polygon);
    }
}