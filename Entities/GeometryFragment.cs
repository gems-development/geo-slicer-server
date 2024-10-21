using NetTopologySuite.Geometries;

namespace Entities
{
    public sealed class GeometryFragment
    {
        public int Id { get; set; }
        public Polygon Fragment { get; set; } = null!;
        public MultiLineString NonRenderingBorder { get; set; } = null!;
        public int GeometryOriginalId { get; set; }
        public GeometryOriginal GeometryOriginal { get; set; } = null!;
    }
}