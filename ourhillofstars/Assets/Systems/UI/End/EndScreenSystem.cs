
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Levels;
using Systems.Levels.Events;
using UniRx;
using UnityEngine;

namespace Systems.UI.End
{
    [GameSystem]
    public class EndScreenSystem : GameSystem<EndScreenComponent>
    {
        public override void Register(EndScreenComponent component)
        {
            component.gameObject.SetActive(false);
            
            MessageBroker.Default.Receive<FinishLastLevelMsg>()
                .Subscribe(_ => RenderEndScreen(component))
                .AddTo(component);
        }

        private void RenderEndScreen(EndScreenComponent component)
        {
            component.gameObject.SetActive(true);
            var levels = IoC.Resolve<LevelSystem.IGetAllLevelsAndGrades>().GetAllLevelsWithGrade();
            foreach (var level in levels)
            {
                CreateGradeElementComponent(component, level.level.LevelNumber, level.grade);
            }
        }

        private void CreateGradeElementComponent(EndScreenComponent component, int levelNumber, Grade grade)
        {
            var levelName = "Level " + levelNumber;
            var gradeElement = Object.Instantiate(component.gradeElementPrefab, component.results.transform);
            gradeElement.name = levelName;
            var gradeElementComponent = gradeElement.GetComponent<GradeElementComponent>();
            gradeElementComponent.levelName.text = levelName;
            gradeElementComponent.gradeImage.sprite = component.gradeSprites[(int)grade];
        }
    }
}