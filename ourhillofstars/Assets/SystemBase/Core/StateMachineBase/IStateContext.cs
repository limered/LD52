using UniRx;

namespace SystemBase.Core
{
    public interface IStateContext<TState, T> where TState : IState<T>
    {
        ReactiveProperty<TState> CurrentState { get; }
        ReactiveCommand<TState> BeforeStateChange { get; }
        ReactiveCommand<TState> AfterStateChange { get; }
        void Start(TState initialState);
        
        bool GoToState(TState state);
    }
}