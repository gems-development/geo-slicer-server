using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Entities
{
    public class GeometryOriginal
    {
        public int Id { get; set; }
        public Polygon Data { get; set; } = null!;
        public virtual ICollection<GeometryFragment> GeometryFragments { get; set; } = null!;
    }
}