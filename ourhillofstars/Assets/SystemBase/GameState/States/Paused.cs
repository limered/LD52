using SystemBase.Core;
using SystemBase.GameState.Messages;
using UniRx;

namespace SystemBase.GameState.States
{
    [NextValidStates(typeof(Running))]
    public class Paused : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgUnpause>()
                .Subscribe(unpause => context.GoToState(new Running()))
                .AddTo(this);
        }
    }
}
