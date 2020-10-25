using Engine.Utils.Base;

namespace Engine.Utils
{
    public class LocatableService<T>
    {
        private T service;

        public LocatableService(T defaultService)
        {
            service = defaultService;
        }

        public void ProvideService(T service) 
        {
            this.service = service;
        }

        public T GetService()
        {
            return service;
        }
    }

    public static class ServiceLocator
    {
        public static LocatableService<BaseRenderer> Renderer = new LocatableService<BaseRenderer>(new NullRenderer());
    }
}
