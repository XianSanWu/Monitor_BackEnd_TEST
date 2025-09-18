using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;

namespace Repository.Implementations
{
    public class RepositoryFactory(IServiceProvider serviceProvider) : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public TRepository Create<TRepository>(params object[] parameters) where TRepository : class
        {
            // 回傳具體類別的實例
            return ActivatorUtilities.CreateInstance<TRepository>(_serviceProvider, parameters);
        }
    }

}
