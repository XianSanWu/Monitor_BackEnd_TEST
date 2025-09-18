using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;

namespace Repository.Implementations
{

    public class RepositoryFactory(IServiceProvider serviceProvider) : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public TRepository Create<TRepository>(params object[] parameters) where TRepository : class
        {
            var type = typeof(TRepository);

            // 嘗試從 DI 容器解析
            var instance = _serviceProvider.GetService<TRepository>();
            if (instance != null) return instance;

            // fallback：如果是 concrete 類別或 interface 指向 concrete，也能用 ActivatorUtilities 建立
            return ActivatorUtilities.CreateInstance<TRepository>(_serviceProvider, parameters);
        }
    }

}
