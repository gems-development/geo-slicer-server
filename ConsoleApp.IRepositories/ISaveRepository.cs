namespace ConsoleApp.IRepositories
{
    public interface ISaveRepository<TK, TKey>
    {
        TKey Save(TK objectToSave);
    }
}