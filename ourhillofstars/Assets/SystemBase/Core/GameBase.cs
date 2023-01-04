using System;
using UniRx;
using UnityEngine;

namespace SystemBase.Core
{
    public class GameBase : MonoBehaviour, IGameSystem
    {
        private readonly GameSystemCollection _systems = new GameSystemCollection(); 
        public StringReactiveProperty DebugMainFrameCallback = new StringReactiveProperty();

        public Type[] ComponentsToRegister => Type.EmptyTypes;

        public virtual void Init()
        {
            _systems.Initialize();
            
            // DontDestroyOnLoad(this);
            
            DebugMainFrameCallback.ObserveOnMainThread()
                .Subscribe(OnDebugCallbackCalled);
        }

        public void RegisterComponent(GameComponent component)
        {
            _systems.RegisterComponent(component);
        }


        public T System<T>() where T : IGameSystem, new()
        {
            return _systems.GetSystem<T>();
        }

        protected virtual void OnDebugCallbackCalled(string s)
        {
            print(s);
        }
    }
}