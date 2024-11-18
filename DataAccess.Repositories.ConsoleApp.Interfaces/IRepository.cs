namespace DataAccess.Repositories.ConsoleApp.Interfaces
{
    public interface IRepository<TInput, TOutput>
    {
        TOutput Save(TInput objectToSave);
    }
}