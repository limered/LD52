using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using UniRx;
using UnityEngine;

namespace Systems.UI
{
    [GameSystem]
    public class PauseMenuSystem : GameSystem<PauseMenuComponent>
    {
        public override void Register(PauseMenuComponent component)
        {
            component.gameObject.SetActive(false);
            
            MessageBroker.Default.Receive<ShowPauseMenuMsg>()
                .Subscribe(_ => HandlePause(component))
                .AddTo(component);
        }

        private static void HandlePause(PauseMenuComponent component)
        {
            var levelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
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