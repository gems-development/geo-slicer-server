namespace DataAccess.Repositories.ConsoleApp.Interfaces;

public interface IRepository<TInput, TOutput>
{
    TOutput Save(TInput objectToSave);
    void StartTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}