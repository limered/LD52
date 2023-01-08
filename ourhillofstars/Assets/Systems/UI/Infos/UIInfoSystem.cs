using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels;
using UniRx;

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
            component.levelName.text = "Level " + level.LevelNumber;
            component.vehicleImage.sprite = component.vehicleSprites[level.playerThemeFile];

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