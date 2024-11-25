namespace Services.Fixers.Interfaces
{
    public interface IConcreteFixer<TGeometry>
    {
        public TGeometry Fix(TGeometry geometry);
    }
}