using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace SystemBase.Utils
{
    public static class IoC
    {
        private static Dictionary<Type, IoCRegister> _registrations = new();

        static IoC()
        {
            RegisterAllRegistrations();
        }

        public static Game Game => Resolve<Game>();

        public static void Reset()
        {
            _registrations = new Dictionary<Type, IoCRegister>();
            RegisterAllRegistrations();
        }

        public static void RegisterType<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            var reg = new IoCRegister
            {
                ResolveType = typeof(TImplementation),
            };

            _registrations.Add(typeof(TInterface), reg);
        }

        public static void RegisterType<TImplementation>()
        {
            var reg = new IoCRegister
            {
                ResolveType = typeof(TImplementation),
            };
            _registrations.Add(typeof(TImplementation), reg);
        }

        public static void RegisterSingleton<TInterface, TImplementation>(bool replaceIfAlreadyRegistered = false)
            where TImplementation : TInterface, new()
        {
            var reg = new IoCRegister
            {
                IsSingleton = true,
                SingletonSubject = new ReactiveProperty<dynamic>(),
                ResolveType = typeof(TImplementation),
            };

            if (replaceIfAlreadyRegistered && _registrations.ContainsKey(typeof(TImplementation)))
            {
                _registrations[typeof(TImplementation)] = reg;
            }
            else
            {
                _registrations.Add(typeof(TImplementation), reg);
            }
        }

        public static void RegisterSingleton<TImplementation>(TImplementation instance,
            bool replaceIfAlreadyRegistered = false)
        {
            var reg = new IoCRegister
            {
                IsSingleton = true,
                SingletonSubject = new ReactiveProperty<dynamic>(instance),
                ResolveType = typeof(TImplementation),
                CachedInstance = instance
            };

            if (replaceIfAlreadyRegistered && _registrations.ContainsKey(typeof(TImplementation)))
            {
                _registrations[typeof(TImplementation)] = reg;
            }
            else
            {
                _registrations.Add(typeof(TImplementation), reg);
            }
        }

        public static void RegisterSingleton<TResolve, TImplementation>(TImplementation instance,
            bool replaceIfAlreadyRegistered = false)
            where TImplementation : TResolve, new()
        {
            var reg = new IoCRegister
            {
                IsSingleton = true,
                SingletonSubject = new ReactiveProperty<dynamic>(instance),
                ResolveType = typeof(TImplementation),
                CachedInstance = instance
            };

            if (replaceIfAlreadyRegistered && _registrations.ContainsKey(typeof(TResolve)))
            {
                _registrations[typeof(TResolve)] = reg;
            }
            else
            {
                _registrations.Add(typeof(TResolve), reg);
            }
        }

        public static void RegisterSingleton<TResolve>(Func<object> lazyInstance,
            bool replaceIfAlreadyRegistered = false)
        {
            var reg = new IoCRegister
            {
                IsSingleton = true,
                SingletonSubject = new ReactiveProperty<dynamic>(),
                CreationFunction = lazyInstance,
            };

            if (replaceIfAlreadyRegistered && _registrations.ContainsKey(typeof(TResolve)))
            {
                _registrations[typeof(TResolve)] = reg;
            }
            else
            {
                _registrations.Add(typeof(TResolve), reg);
            }
        }

        public static void Overwrite<TResolve>(object replacement)
        {
            var reg = _registrations[typeof(TResolve)];
            reg.IsOverwritten = true;
            reg.OverwriteInstance = replacement;
        }

        public static void RemoveOverwrite<TResolve>()
        {
            var reg = _registrations[typeof(TResolve)];
            reg.IsOverwritten = false;
            reg.OverwriteInstance = null;
        }

        public static TResolve Resolve<TResolve>()
        {
            if (!_registrations.TryGetValue(typeof(TResolve), out var reg))
            {
                throw new KeyNotFoundException("Unknown interface: " + typeof(TResolve).FullName);
            }

            if (!reg.IsSingleton)
            {
                return (TResolve)ResolveType(reg);
            }

            return (TResolve)ResolveSingleton(reg);
        }

        private static void RegisterAllRegistrations()
        {
            var interfaceType = typeof(IIocRegistration);

            var registrations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(assemblyType => interfaceType.IsAssignableFrom(assemblyType) && assemblyType.IsClass);

            foreach (var registrationType in registrations)
            {
                var registration = Activator.CreateInstance(registrationType) as IIocRegistration;
                registration?.Register();
            }
        }

        private static object ResolveSingleton(IoCRegister reg)
        {
            if (reg.IsOverwritten)
            {
                return reg.OverwriteInstance;
            }

            if (reg.CachedInstance != null) return reg.CachedInstance;

            reg.CachedInstance = reg.CreationFunction == null
                ? Activator.CreateInstance(reg.ResolveType)
                : reg.CreationFunction();

            reg.SingletonSubject.Value = reg.CachedInstance;

            return reg.CachedInstance;
        }

        private static object ResolveType(IoCRegister reg)
        {
            return reg.IsOverwritten ? reg.OverwriteInstance : Activator.CreateInstance(reg.ResolveType);
        }
    }

    internal class IoCRegister
    {
        public Type ResolveType { get; set; }
        public bool IsSingleton { get; set; }
        public ReactiveProperty<dynamic> SingletonSubject { get; set; }
        public object CachedInstance { get; set; }
        public Func<object> CreationFunction { get; set; }
        public bool IsOverwritten { get; set; }
        public object OverwriteInstance { get; set; }
    }

    public interface IIocRegistration
    {
        void Register();
    }
}