using SystemBase.Core;
using SystemBase.GameState.Messages;
using UniRx;

namespace SystemBase.GameState.States
{
    [NextValidStates(typeof(GameOver), typeof(Paused))]
    public class Running : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgEnd>()
                .Subscribe(end => context.GoToState(new GameOver()))
                .AddTo(this);

            MessageBroker.Default.Receive<GameMsgPause>()
                .Subscribe(pause => context.GoToState(new Paused()))
                .AddTo(this);
        }
    }
}
