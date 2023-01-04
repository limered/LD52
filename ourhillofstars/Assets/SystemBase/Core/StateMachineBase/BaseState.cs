using System;
using System.Collections.ObjectModel;
using UniRx;

namespace SystemBase.Core
{
    public abstract class BaseState<T> : IState<T>, IDisposable
    {
        // ReSharper disable once InconsistentNaming
        public readonly CompositeDisposable StateDisposables = new CompositeDisposable();

        protected BaseState()
        {
            if (Attribute.GetCustomAttribute(GetType(), typeof(NextValidStatesAttribute)) is NextValidStatesAttribute
                definedAttribute)
            {
                ValidNextStates = new ReadOnlyCollection<Type>(definedAttribute.ValidStateChanges);
            }
        }

        public ReadOnlyCollection<Type> ValidNextStates { get; private set; }

        public void Dispose()
        {
            if (StateDisposables.IsDisposed) return;

            StateDisposables.Dispose();
        }

        public abstract void Enter(StateContext<T> context);

        public void Enter(IStateContext<IState<T>, T> context)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            Enter(context as StateContext<T>);
        }

        public virtual bool Exit()
        {
            Dispose();
            return true;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}