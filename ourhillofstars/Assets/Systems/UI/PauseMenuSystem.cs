using SystemBase.Core;
using UniRx;

namespace Systems.UI
{
    [GameSystem]
    public class PauseMenuSystem : GameSystem<PauseMenuComponent>
    {
        public override void Register(PauseMenuComponent component)
        {
            MessageBroker.Default.Receive<ShowPauseMenuMsg>()
                .Subscribe(_ => component.gameObject.SetActive(true))
                .AddTo(component);
        }
    }
}