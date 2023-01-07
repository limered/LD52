using SystemBase.Core;
<<<<<<< Updated upstream
using Systems.Levels;
=======
>>>>>>> Stashed changes
using UniRx;

namespace Systems.UI
{
    [GameSystem]
    public class PauseMenuSystem : GameSystem<PauseMenuComponent>
    {
        public override void Register(PauseMenuComponent component)
        {
<<<<<<< Updated upstream
            MessageBroker.Default.Receive<ShowPauseMenuMsg>()
                .Subscribe(_ => component.gameObject.SetActive(true))
                .AddTo(component);
=======
            MessageBroker.Default.Publish<ShowLevelOverwievMessage>();
>>>>>>> Stashed changes
        }
    }
}