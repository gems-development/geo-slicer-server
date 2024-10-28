using System.Collections.Generic;

namespace ISlicers
{
    public interface ISlicer<TK, TV>
    {
        IEnumerable<TV> Slice(TK polygon);
    }
}