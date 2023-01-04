using System;

namespace SystemBase.Core
{
    public interface IGameSystem
    {
        Type[] ComponentsToRegister { get; }

        void Init();

        void RegisterComponent(GameComponent component);
    }
}
