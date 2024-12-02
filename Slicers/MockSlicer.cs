using System.Collections.Generic;
using Slicers.Interfaces;

namespace Slicers
{
    public class MockSlicer<TGeometry> : ISlicer<TGeometry, TGeometry>
    {
        public IEnumerable<TGeometry> Slice(TGeometry polygon)
        {
            return new[] { polygon };
        }
    }
}