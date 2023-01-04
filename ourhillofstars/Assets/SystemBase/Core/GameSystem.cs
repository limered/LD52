using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
// ReSharper disable UnusedType.Global

namespace SystemBase.Core
{
    public abstract class GameSystem<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>
         : GameSystem<TComponent1, TComponent2, TComponent3, TComponent4>
        where TComponent1 : GameComponent
        where TComponent2 : GameComponent
        where TComponent3 : GameComponent
        where TComponent4 : GameComponent
        where TComponent5 : GameComponent
    {
        public override Type[] ComponentsToRegister => new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3), typeof(TComponent4), typeof(TComponent5) };

        public abstract void Register(TComponent5 component);
    }

    public abstract class GameSystem<TComponent1, TComponent2, TComponent3, TComponent4>
        : GameSystem<TComponent1, TComponent2, TComponent3>
        where TComponent1 : GameComponent
        where TComponent2 : GameComponent
        where TComponent3 : GameComponent
        where TComponent4 : GameComponent
    {
        public override Type[] ComponentsToRegister => new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3), typeof(TComponent4) };

        public abstract void Register(TComponent4 component);
    }

    public abstract class GameSystem<TComponent1, TComponent2, TComponent3>
        : GameSystem<TComponent1, TComponent2>
        where TComponent1 : GameComponent
        where TComponent2 : GameComponent
        where TComponent3 : GameComponent
    {
        public override Type[] ComponentsToRegister => new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3) };

        public abstract void Register(TComponent3 component);
    }

    public abstract class GameSystem<TComponent1, TComponent2>
        : GameSystem<TComponent1>
        where TComponent1 : GameComponent
        where TComponent2 : GameComponent
    {
        public override Type[] ComponentsToRegister => new[] { typeof(TComponent1), typeof(TComponent2) };

        public abstract void Register(TComponent2 component);
    }

    public abstract class GameSystem<TComponent> : GameSystem where TComponent : GameComponent
    {
        private Dictionary<Type, Action<GameComponent>> _registerMethods;

        public override Type[] ComponentsToRegister => new[] { typeof(TComponent) };

        public override void RegisterComponent(GameComponent component)
        {
            if (_registerMethods == null)
            {
                _registerMethods = new Dictionary<Type, Action<GameComponent>>();
                var methods = GetType().GetMethods();

                foreach (var m in methods.Where(m => m.Name == "Register" && m.GetParameters().Length == 1))
                {
                    // ReSharper disable once AccessToForEachVariableInClosure
                    _registerMethods.Add(m.GetParameters()[0].ParameterType, c => m.Invoke(this, new object[] { c }));
                }
            }

            if (_registerMethods.ContainsKey(component.GetType())) _registerMethods[component.GetType()](component);
            else Debug.LogError(GetType().Name + ": No Register-Method found for '" + component.GetType().Name + "'");
        }

        public abstract void Register(TComponent component);
    }

    public abstract class GameSystem : IGameSystem
    {
        public virtual Type[] ComponentsToRegister { get; }

        protected GameSystem()
        {
            ComponentsToRegister = Type.EmptyTypes;
        }

        public virtual void Init() { }

        public abstract void RegisterComponent(GameComponent component);

        public IObservable<float> SystemUpdate()
        {
            return IoC.Game.UpdateAsObservable().Select(_ => Time.deltaTime);
        }

        public IObservable<float> SystemFixedUpdate()
        {
            return IoC.Game.FixedUpdateAsObservable().Select(_ => Time.deltaTime);
        }

        public IObservable<float> SystemLateUpdate()
        {
            return IoC.Game.LateUpdateAsObservable().Select(_ => Time.deltaTime);
        }

        protected static IObservable<TComp> SystemUpdate<TComp>(TComp returnedComponent)
            where TComp : GameComponent
        {
            return IoC.Game.UpdateAsObservable().Select(_ => returnedComponent);
        }

        protected static IObservable<TComp> SystemFixedUpdate<TComp>(TComp returnedComponent)
            where TComp : GameComponent
        {
            return IoC.Game.UpdateAsObservable().Select(_ => returnedComponent);
        }

        protected static IObservable<TComp> SystemLateUpdate<TComp>(TComp returnedComponent)
            where TComp : GameComponent
        {
            return IoC.Game.UpdateAsObservable().Select(_ => returnedComponent);
        }

        #region Lazy Component registration

        private readonly Dictionary<Type, ReactiveProperty<dynamic>> _lazy = new();

        public AfterTheComponentIsAvailable<T> WaitOn<T>() where T : GameComponent
        {
            if (!_lazy.ContainsKey(typeof(T)))
            {
                _lazy[typeof(T)] = new ReactiveProperty<dynamic>();
            }
            return new AfterTheComponentIsAvailable<T>(_lazy[typeof(T)]
                .Select(x => x as T).WhereNotNull().First());
        }

        protected void RegisterWaitable<T>(T comp) where T : GameComponent
        {
            if (_lazy.ContainsKey(typeof(T)))
            {
                _lazy[typeof(T)].Value = comp;
            }
            else
            {
                _lazy[typeof(T)] = new ReactiveProperty<dynamic>(comp);
            }
        }

        #endregion Lazy Component registration
    }
}