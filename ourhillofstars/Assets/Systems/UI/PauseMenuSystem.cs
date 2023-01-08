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
                .Subscribe(_ => HandlePause(component))
                .AddTo(component);
            component.isPaused.Value = false;
            component.gameObject.SetActive(component.isPaused.Value);
        }

        private static void HandlePause(PauseMenuComponent component)
        {
            component.gameObject.SetActive(!component.isPaused.Value);
            component.isPaused.Value = !component.isPaused.Value;
        }
    }
}