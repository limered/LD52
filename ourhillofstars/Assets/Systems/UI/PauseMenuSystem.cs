using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
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
            currentLevelComponent.IsPaused.Value = false;
            component.gameObject.SetActive(currentLevelComponent.IsPaused.Value);
        }

        private static void HandlePause(PauseMenuComponent component, CurrentLevelComponent levelComponent)
        {
            switch (levelComponent.GameState)
            {
                case GameState.GameState.Paused:
                    component.gameObject.SetActive(false);
                    levelComponent.IsPaused.Value = false;
                    levelComponent.GameState = GameState.GameState.Playing;
                    break;
                case  GameState.GameState.Playing:
                    component.gameObject.SetActive(true);
                    levelComponent.IsPaused.Value = true;
                    levelComponent.GameState = GameState.GameState.Paused;
                    break;
                case  GameState.GameState.LevelSelect:
                    component.gameObject.SetActive(!component.gameObject.activeSelf);
                    levelComponent.IsPaused.Value = true;
                    levelComponent.GameState = GameState.GameState.LevelSelect;
                    break;
            }
        }
    }
}