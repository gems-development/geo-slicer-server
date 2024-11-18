using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ConsoleApp.IRepositories;
using DataAccess.Interfaces;
using DomainModels;
using Entities;
using GeoSlicer.Utils;
using NetTopologySuite.Geometries;
using Utils;

namespace ConsoleApp.Repositories
{
    public class SaveRepository : ISaveRepository<GeometryWithFragments<Polygon, Polygon>, int>
    {
        private readonly GeometryDbContext _dbContext;

        private double _epsilon = 1E-14;

        public SaveRepository(GeometryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public int Save(GeometryWithFragments<Polygon, Polygon> objectToSave)
        {
            GeometryOriginal geometryOriginal = new GeometryOriginal
            {
                Data = objectToSave.Data
            };
            _dbContext.GeometryOriginals.Add(geometryOriginal);

            IEnumerable<LineString> originalLines = PolygonUtils.GetAllLines(objectToSave.Data);

            CoordinateSequence[] allRingsCoordinates =
                new[] { objectToSave.Data.Shell.CoordinateSequence }
                    .Concat(objectToSave.Data.Holes.Select(ring => ring.CoordinateSequence)).ToArray();

            IEnumerable<GeometryFragment> fragments = objectToSave.GeometryFragments.Select(polygon =>
                new GeometryFragment
                {
                    Fragment = polygon,
                    GeometryOriginal = geometryOriginal,

                    // Ищем, какие отрезки внутренние (не должны отображаться)
                    NonRenderingBorder = new MultiLineString(
                        // Преобразуем полигон в набор отрезков
                        PolygonUtils.GetAllLines(polygon).Where(line =>
                        {
                            // Если текущий отрезок полностью равен одному из отрезков оригинала
                            if (originalLines.Contains(line))
                            {
                                return false;
                            }

                            double[] xs = line.GetOrdinates(Ordinate.X);
                            double[] ys = line.GetOrdinates(Ordinate.Y);

                            // Ищем по оболочке и всем дырам оригинала
                            foreach (CoordinateSequence sequence in allRingsCoordinates)
                            {
                                for (int i = 0; i < sequence.Count - 1; i++)
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
                                }
                            }

                            return true;
                        }).ToArray())
                });


            _dbContext.GeometryFragments.AddRange(fragments);
            _dbContext.SaveChanges();
            return geometryOriginal.Id;
        }
    }
}