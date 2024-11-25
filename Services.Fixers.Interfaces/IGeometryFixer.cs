namespace Services.Interfaces
using System.Collections.Generic;
using System.Linq;
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