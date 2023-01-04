using System;
using UniRx;
using UniRx.Triggers;

namespace SystemBase.Core
{
    public class AfterTheComponentIsAvailable<T> : IObservable<T> where T : GameComponent
    {
        private readonly IObservable<T> _lazy;

        public AfterTheComponentIsAvailable(IObservable<T> lazy)
        {
            _lazy = lazy;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _lazy.Subscribe(observer);
        }

        public IObservable<TU> Then<TU>(Func<T, IObservable<TU>> then)
        {
            return _lazy.SelectMany(then);
        }

        public IDisposable ThenOnUpdate(Action<T> everyFrame)
        {
            return Then(x => x.UpdateAsObservable().Select(_ => x)).Subscribe(everyFrame);
        }
    }
}