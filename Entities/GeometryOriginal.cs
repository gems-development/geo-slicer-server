using NetTopologySuite.Geometries;

namespace Entities;

public class GeometryOriginal
{
    public int Id { get; set; }
    public string Properties { get; set; } = null!;
    public Geometry Data { get; set; } = null!;
    public virtual ICollection<GeometryFragment> GeometryFragments { get; set; } = null!;
    public virtual Layer Layer { get; set; } = null!;
    public int LayerId { get; set; }
}