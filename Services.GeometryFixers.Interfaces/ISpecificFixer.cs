namespace Services.GeometryFixers.Interfaces
{
    public interface ISpecificFixer<TGeometry>
    {
        public TGeometry Fix(TGeometry geometry);
    }
}