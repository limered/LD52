
using SystemBase.Core;
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
        }

        private void CreateGradeElementComponent(EndScreenComponent component)
        {
            var levelName = "Level "; //TODO levelnumber
            var gradeElement = Object.Instantiate(component.gradeElementPrefab, component.results.transform);
            gradeElement.name = levelName;
            var gradeElementComponent = gradeElement.GetComponent<GradeElementComponent>();
            gradeElementComponent.levelName.text = levelName;
            gradeElementComponent.gradeImage.sprite = component.gradeSprites[0]; //TODO get grade and map to int
        }
    }
}