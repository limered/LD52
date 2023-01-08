using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels;
using UniRx;
using UnityEngine;

namespace Systems.UI.Infos
{
    [GameSystem]
    public class UIInfoSystem : GameSystem<UIInfoComponent>
    {
        public override void Register(UIInfoComponent component)
        {
            MessageBroker.Default.Receive<GridLoadMsg>()
                .Subscribe(msg => LoadGameInfoForUI(component, msg.Level))
                .AddTo(component);
        }

        private void LoadGameInfoForUI(UIInfoComponent component, Level level)
        {
            component.levelName.text = "Level " + (level.LevelIndex + 1);
            component.vehicleImage.sprite = component.vehicleSprites[level.playerThemeFile];
            Debug.Log("load theme file " + level.themeFile);
            component.harvestItemImage.sprite = component.harvestSprites[level.themeFile];

            IoC.Game.GetComponent<CurrentLevelComponent>().arrowsUsed
                .Subscribe(_ =>
                {
                    var currentGame = IoC.Game.GetComponent<CurrentLevelComponent>();
                    
                    //TODO render stars
                    component.grade.text = "Grade: " + currentGame.CurrentGrade;
                })
                .AddTo(component);
        }
    }
}