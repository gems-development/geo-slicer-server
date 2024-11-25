namespace IGeometryFixers
{
    public interface IConcreteFixer<TGeometry>
    {
        public TGeometry Fix(TGeometry geometry);
    }
}