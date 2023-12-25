namespace AirportSimulator.API.Logic
{
    public interface ILogic<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> NewEntry(int type);
        Task<bool> ReadyToMove(T entity);
        Task<T> Move(T entity);
    }
}
