namespace DataAccess.Repositories.ConsoleApp.Interfaces;

public interface IRepository<TInput, TOutput>
{
    TOutput Save(TInput objectToSave, string layerAlias, int srid);
    void StartTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}