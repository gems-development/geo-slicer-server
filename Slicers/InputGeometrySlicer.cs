using System;
using System.Collections.Generic;
using GeoSlicer.GridSlicer;
using GeoSlicer.Utils;
using GeoSlicer.Utils.Intersectors;
using GeoSlicer.Utils.Intersectors.CoordinateComparators;
using GeoSlicer.Utils.PolygonClippingAlghorithm;
using NetTopologySuite.Geometries;
using Slicers.Interfaces;

namespace Slicers
{
    public class InputGeometrySlicer : ISlicer<Polygon, Polygon>
    {
        private Slicer _currentSlicer;
        private double _xScale;
        private double _yScale;
        private bool _uniqueOnly;
        
        public InputGeometrySlicer(double xScale,
                                    double yScale,
                                    bool uniqueOnly = true)
        {
            const double epsilon = 1E-15;
            const double epsilon2 = 1E-8;

            EpsilonCoordinateComparator coordinateComparator = new EpsilonCoordinateComparator(epsilon2);
            LineService lineService = new LineService(epsilon, coordinateComparator);

            WeilerAthertonAlghorithm weilerAthertonAlghorithm = new WeilerAthertonAlghorithm(
                new LinesIntersector(coordinateComparator, lineService, epsilon), lineService,
                coordinateComparator, new ContainsChecker(lineService, epsilon), epsilon);
            
            _currentSlicer = new Slicer(weilerAthertonAlghorithm);
            
            _xScale = xScale;
            _yScale = yScale;
            _uniqueOnly = uniqueOnly;
        }

        public IEnumerable<Polygon> Slice(Polygon polygon)
        {
            IEnumerable<Polygon>?[,] result = _currentSlicer.Slice(polygon, _xScale, _yScale, _uniqueOnly);

            List<Polygon> allPolygons = new List<Polygon>();

            foreach (var listPolygons in result)
            {
                if (listPolygons != null)
                {
                    allPolygons.AddRange(listPolygons);
                }
            }

            return allPolygons;
        }
    }
}