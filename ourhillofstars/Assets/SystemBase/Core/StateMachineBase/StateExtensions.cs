using System;

namespace SystemBase.Core
{
    public static class StateExtensions
    {
        public static TDisposable AddTo<TDisposable, T>(this TDisposable dis,
            BaseState<T> baseState) where TDisposable : IDisposable
        {
            baseState.StateDisposables.Add(dis);
            return dis;
        }
    }
}