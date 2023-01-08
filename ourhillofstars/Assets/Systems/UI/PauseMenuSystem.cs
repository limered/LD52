using SystemBase.Core;
using SystemBase.Utils;
using Systems.Levels;
using UniRx;

namespace Systems.UI
{
    [GameSystem]
    public class PauseMenuSystem : GameSystem<PauseMenuComponent>
    {
        public override void Register(PauseMenuComponent component)
        {
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            MessageBroker.Default.Receive<ShowPauseMenuMsg>()
                .Subscribe(_ => HandlePause(component, currentLevelComponent))
                .AddTo(component);
            currentLevelComponent.IsPaused = false;
            component.gameObject.SetActive(currentLevelComponent.IsPaused);
        }

        private static void HandlePause(PauseMenuComponent component, CurrentLevelComponent levelComponent)
        {
            component.gameObject.SetActive(!levelComponent.IsPaused);
            levelComponent.IsPaused = !levelComponent.IsPaused;
        }
    }
}