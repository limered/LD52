using SystemBase.Utils;

namespace SystemBase.Adapter
{
    public class IoCAdapterRegistrations : IIocRegistration
    {
        public void Register()
        {
            IoC.RegisterType<IRandom, RandomAdapter>();
            IoC.RegisterType<IMathf, MathfAdapter>();
        }
    }
}