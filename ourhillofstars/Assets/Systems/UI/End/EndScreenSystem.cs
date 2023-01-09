using SystemBase.Core;
using SystemBase.Utils;
using Systems.Levels;
using Systems.Levels.Events;
using Systems.UI.Levels;
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
            var levels = IoC.Resolve<LevelOverviewSystem.IGetAllLevelsAndGrades>().GetAllLevelsWithGrade();
            foreach (var level in levels)
            {
                CreateGradeElementComponent(component, level.level.LevelIndex, level.grade);
            }
        }

        private void CreateGradeElementComponent(EndScreenComponent component, int levelIndex, Grade grade)
        {
            var levelName = "Level " + (levelIndex + 1);
            var gradeElement = Object.Instantiate(component.gradeElementPrefab, component.results.transform);
            gradeElement.name = levelName;
            var gradeElementComponent = gradeElement.GetComponent<GradeElementComponent>();
            gradeElementComponent.levelName.text = levelName;
            gradeElementComponent.gradeImage.sprite = grade == Grade.None ?
                component.gradeSprites[3] : component.gradeSprites[(int)grade];
        }
    }
}