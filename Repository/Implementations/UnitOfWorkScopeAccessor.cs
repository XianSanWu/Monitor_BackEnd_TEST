using Repository.Interfaces;

namespace Repository.Implementations
{
    public class UnitOfWorkScopeAccessor : IUnitOfWorkScopeAccessor
    {
        // 每個 async context(request/thread) 都有自己的 Current
        private static readonly AsyncLocal<IUnitOfWork?> _current = new();

        public IUnitOfWork? Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }
}
