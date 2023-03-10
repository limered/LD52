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
            component.levelName.text = "Level " + (level.LevelIndex + 1);
            component.vehicleImage.sprite = component.vehicleSprites[level.playerThemeFile];
            component.harvestItemImage.sprite = component.harvestSprites[level.themeFile];
            SetGradeSprite(component, Grade.None);

            IoC.Game.GetComponent<CurrentLevelComponent>().arrowsUsed
                .Subscribe(_ =>
                {
                    var currentGame = IoC.Game.GetComponent<CurrentLevelComponent>();
                    SetGradeSprite(component, currentGame.CurrentGrade);
                })
                .AddTo(component);
        }

        private void SetGradeSprite(UIInfoComponent component, Grade grade)
        {
            component.grade.sprite = grade == Grade.None ?
                component.gradeSprites[0] : component.gradeSprites[(int)grade];
        }
    }
}