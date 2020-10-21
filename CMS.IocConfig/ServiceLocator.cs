using Microsoft.Extensions.DependencyInjection;

namespace CMS.IocConfig
{
    public class ServiceLocator 
    {
        public static ServiceLocatorAdapter Current;
        public ServiceLocator(IServiceCollection serviceDescriptors) 
        {
            Current = new ServiceLocatorAdapter(serviceDescriptors);
        }
    }
    public class ServiceLocatorAdapter 
    {
        private readonly ServiceProvider _serviceProvider;
        public ServiceLocatorAdapter(IServiceCollection serviceDescriptors) 
        {
            _serviceProvider = serviceDescriptors.BuildServiceProvider();
        }
        public T GetInstance<T>() 
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
