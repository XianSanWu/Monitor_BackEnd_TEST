
namespace Repository.Interfaces
{
    public interface IRepositoryFactory
    {
        TRepository Create<TRepository>(params object[] parameters) where TRepository : class;
    }

}
