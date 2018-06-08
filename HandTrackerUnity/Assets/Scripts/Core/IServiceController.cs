using Scripts.Core.Services;

namespace Scripts.Core
{
    public interface IServiceController
    {
		void Initialization();
        bool ServicesInitialized { get; }
        TService GetService<TService>() where TService : class;
    }
}