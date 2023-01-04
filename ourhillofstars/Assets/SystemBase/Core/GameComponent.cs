using System;
using System.Collections.Generic;
using SystemBase.Utils;
using UniRx;
using UnityEngine;

namespace SystemBase.Core
{
    public class GameComponent : MonoBehaviour, IGameComponent
    {
        public List<IDisposable> ComponentDisposables = new List<IDisposable>();
        public virtual IGameSystem System { get; set; }

        public void RegisterToGame()
        {
            IoC.Resolve<Game>().RegisterComponent(this);
        }

        public IObservable<TComponent> WaitOn<TComponent>(ReactiveProperty<TComponent> componentToWaitOnTo)
            where TComponent : GameComponent
        {
            return componentToWaitOnTo.WhereNotNull().Select(waitedComponent => waitedComponent);
        }

        public IDisposable WaitOn<TComponent>(ReactiveProperty<TComponent> componentToWaitOnTo, Action<TComponent> onNext)
            where TComponent : GameComponent
        {
            return componentToWaitOnTo
                .WhereNotNull()
                .Select(waitedComponent => waitedComponent)
                .Subscribe(onNext)
                .AddTo(this);
        }

        public T AddDisposablele<T>(T disposable) where T : IDisposable
        {
            ComponentDisposables.Add(disposable);
            return disposable;
        }

        protected void Start()
        {
            RegisterToGame();

            OverwriteStart();
        }

        protected virtual void OverwriteStart()
        {
        }

        protected void OnDestroy()
        {
            ComponentDisposables.ForEach(disposable => disposable.Dispose());
        }
    }

    public class SemanticGameComponent<TGameComponent> : GameComponent where TGameComponent : IGameComponent
    {
        public TGameComponent dependency;

        public TGameComponent Dependency
        {
            get => dependency != null ? dependency : GetComponent<TGameComponent>();
            set => dependency = value;
        }
    }
}
