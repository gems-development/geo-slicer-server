using NetTopologySuite.Geometries;

namespace Entities;

public class GeometryFragment
{
    public int Id { get; set; }
    public Geometry Fragment { get; set; } = null!;
    public Geometry NonRenderingBorder { get; set; } = null!;
    public int GeometryOriginalId { get; set; }
    public virtual GeometryOriginal GeometryOriginal { get; set; } = null!;
}