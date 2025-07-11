using System.Diagnostics.CodeAnalysis;
using DomainModels;
using NetTopologySuite.Geometries;
using Services.GeometrySlicers.Interfaces;
using Utils;
using GeoSlicer.Utils;

namespace Services.GeometrySlicers;

public class GeometryWithFragmentsGeometrySlicer : IGeometrySlicer<Polygon, FragmentWithNonRenderingBorder<Polygon, MultiLineString>>
{
    private readonly IGeometrySlicer<Polygon, Polygon> _geometrySlicer;
        
    private readonly double _epsilon;

    public GeometryWithFragmentsGeometrySlicer(IGeometrySlicer<Polygon, Polygon> geometrySlicer, double epsilon)
    {
        _geometrySlicer = geometrySlicer;
        _epsilon = epsilon;
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public IEnumerable<FragmentWithNonRenderingBorder<Polygon, MultiLineString>> Slice(Polygon polygon)
    {
        IEnumerable<Polygon> polygonFragments = _geometrySlicer.Slice(polygon);
        
        ISet<LineString> originalLines = PolygonUtils.GetAllLinesSet(polygon, 0);

        CoordinateSequence[] allRingsCoordinates =
            new[] { polygon.Shell.CoordinateSequence }
                .Concat(polygon.Holes.Select(ring => ring.CoordinateSequence)).ToArray();
        List<Task<FragmentWithNonRenderingBorder<Polygon, MultiLineString>>> tasks = new();
        foreach (var fragment in polygonFragments)
        {
            var task = Task.Run(() =>
            {
                return new FragmentWithNonRenderingBorder<Polygon, MultiLineString>
                {
                    Fragment = fragment,
                    // Ищем, какие отрезки внутренние (не должны отображаться)
                    NonRenderingBorder = new MultiLineString(
                        // Преобразуем полигон в набор отрезков
                        PolygonUtils.GetAllLines(fragment, 0).Where(line =>
                        {
                            // Если текущий отрезок полностью равен одному из отрезков оригинала
                            if (originalLines.Contains(line) ||
                                originalLines.Contains(new LineString([line.Coordinates[1], line.Coordinates[0]])
                                    { SRID = 0 }))
                            {
                                return false;
                            }

                            double[] xs = line.GetOrdinates(Ordinate.X);
                            double[] ys = line.GetOrdinates(Ordinate.Y);
                            double[] xs1 = [xs[1], xs[0]];
                            double[] ys1 = [ys[1], ys[0]];

                            // Ищем по оболочке и всем дырам оригинала
                            foreach (CoordinateSequence sequence in allRingsCoordinates)
                            {
                                for (int i = 0; i < sequence.Count - 1; i++)
                                {
                                    if (!CheckLine(sequence, i, xs, ys))
                                        return false;

                                    if (!CheckLine(sequence, i, xs1, ys1))
                                        return false;
                                }
                            }

                            return true;
                        }).ToArray())
                };
            });
            tasks.Add(task);
        }

        var fragments = Task.WhenAll(tasks).GetAwaiter().GetResult();
        return fragments;
    }

    private bool CheckLine(CoordinateSequence sequence, int i, double[] xs, double[] ys)
    {
        // Если левая точка отрезка в точности присутствует в оригинале (в проверяемом
        // на данный момент кольце)
        if (sequence.GetOrdinate(i, Ordinate.X) == xs[0] &&
            sequence.GetOrdinate(i, Ordinate.Y) == ys[0])
        {
            // Если при этом правая находится на соответствующем отрезке оригинала
            // (является точкой его разрезания)
            if (Math.Abs(LineService.VectorProduct(xs[1] - xs[0],
                    ys[1] - ys[0],
                    sequence.GetOrdinate(i + 1, Ordinate.X) -
                    sequence.GetOrdinate(i, Ordinate.X),
                    sequence.GetOrdinate(i + 1, Ordinate.Y) -
                    sequence.GetOrdinate(i, Ordinate.Y))) < _epsilon)
            {
                return false;
            }
        }

        // Симметрично
        if (sequence.GetOrdinate(i, Ordinate.X) == xs[1] &&
            sequence.GetOrdinate(i, Ordinate.Y) == ys[1])
        {
            int prevNum = i == 0 ? sequence.Count - 2 : i - 1;
            if (Math.Abs(LineService.VectorProduct(xs[1] - xs[0],
                    ys[1] - ys[0],
                    sequence.GetOrdinate(i, Ordinate.X) -
                    sequence.GetOrdinate(prevNum, Ordinate.X),
                    sequence.GetOrdinate(i, Ordinate.Y) -
                    sequence.GetOrdinate(prevNum, Ordinate.Y))) < _epsilon)
            {
                return false;
            }
        }
        return true;
    }
}