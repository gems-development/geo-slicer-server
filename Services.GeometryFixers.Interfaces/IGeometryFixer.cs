using System.Linq;
using Services.GeometryValidateErrors;

namespace Services.GeometryFixers.Interfaces
{
    public abstract class IGeometryFixer<TGeometry>
    {
        public TGeometry FixGeometry(TGeometry geometry, GeometryValidateError[] geometryValidateErrors)
        {
            if (geometryValidateErrors.All(error => error == GeometryValidateError.GeometryValid))
                return geometry;
            
            geometryValidateErrors =
                geometryValidateErrors.Where(a => a != GeometryValidateError.GeometryValid).ToArray();
            
            return Fix(geometry, geometryValidateErrors);
        }

        protected abstract TGeometry Fix(TGeometry geometry, GeometryValidateError[] geometryValidateErrors);
    }
}