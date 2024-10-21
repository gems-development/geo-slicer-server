using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Entities
{
    public sealed class GeometryOriginal
    {
        public int Id { get; set; }
        public Polygon Data { get; set; } = null!;
        public ICollection<GeometryFragment> GeometryFragments { get; set; } = null!;
    }
}