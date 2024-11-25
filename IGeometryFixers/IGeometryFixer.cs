using System;
using System.Collections.Generic;
using System.Linq;
using GeometryValidateErrors;
namespace IGeometryFixers
{
    public abstract class IGeometryFixer<TGeometry>
    {
        public TGeometry FixGeometry(TGeometry geometry, GeometryValidateError[] geometryValidateErrors)
        {
            if (geometryValidateErrors.All(error => error == GeometryValidateError.GeometryValid))
                return geometry;
            //удалить валидные из массива ошибок
            return Fix(geometry, geometryValidateErrors);
        }

        protected abstract TGeometry Fix(TGeometry geometry, GeometryValidateError[] geometryValidateErrors);
    }
}