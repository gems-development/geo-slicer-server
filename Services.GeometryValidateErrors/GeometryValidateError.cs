namespace Services.GeometryValidateErrors;

public enum GeometryValidateError
{
    GeometryValid = -30,
    GeometryHasRepeatingPoints = -20,
    UnknownError = -10,
        
    /// <summary>
    /// Indicates that a hole of a polygon lies partially
    /// or completely in the exterior of the shell.
    /// </summary>
    HoleOutsideShell = 2,
    /// <summary>
    /// Indicates that a hole lies
    /// in the interior of another hole in the same polygon.
    /// </summary>
    NestedHoles = 3,
    /// <summary>
    /// Indicates that the interior of a polygon is disjoint
    /// (often caused by set of contiguous holes splitting
    /// the polygon into two parts).
    /// </summary>
    DisconnectedInteriors = 4,
    /// <summary>
    /// Indicates that two rings of a polygonal geometry intersect.
    /// </summary>
    SelfIntersection = 5,
    /// <summary>Indicates that a ring self-intersects.</summary>
    RingSelfIntersection = 6,
    /// <summary>
    /// Indicates that a polygon component of a
    /// <see cref="T:NetTopologySuite.Geometries.MultiPolygon" /> lies inside another polygonal component.
    /// </summary>
    NestedShells = 7,
    /// <summary>
    /// Indicates that a polygonal geometry
    /// contains two rings which are identical.
    /// </summary>
    DuplicateRings = 8,
    /// <summary>
    /// Indicates that either:
    /// - A <see cref="T:NetTopologySuite.Geometries.LineString" /> contains a single point.
    /// - A <see cref="T:NetTopologySuite.Geometries.LinearRing" /> contains 2 or 3 points.
    /// </summary>
    TooFewPoints = 9,
    /// <summary>
    /// Indicates that the <c>X</c> or <c>Y</c> ordinate of
    /// a <see cref="T:NetTopologySuite.Geometries.Coordinate" /> is not a valid
    /// numeric value (e.g. <see cref="F:System.Double.NaN" />).
    /// </summary>
    InvalidCoordinate = 10,
    /// <summary>
    /// Indicates that a ring is not correctly closed
    /// (the first and the last coordinate are different).
    /// </summary>
    RingNotClosed = 11
}