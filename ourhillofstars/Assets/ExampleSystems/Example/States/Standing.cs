using System;
using SystemBase.Core;
using UniRx;

namespace ExampleSystems.Example.States
{
    [NextValidStates(typeof(Moving))]
    public class Standing : BaseState<FunnyMovementComponent>
    {
        public override void Enter(StateContext<FunnyMovementComponent> context)
        {
            Observable.Timer(TimeSpan.FromSeconds(3))
                .Subscribe(l => context.GoToState(new Moving()))
                .AddTo(this);
        }
    }
}
