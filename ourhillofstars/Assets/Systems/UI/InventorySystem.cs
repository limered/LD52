using SystemBase.Core;
using SystemBase.Utils;
using Systems.Drescher;
using Systems.GameState;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.UI
{
    [GameSystem]
    public class InventorySystem : GameSystem<InventoryComponent>
    {
        public override void Register(InventoryComponent component)
        {
            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Subscribe(_ => InitArrows(component))
                .AddTo(component);

            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning
                .Subscribe(b => SetButtonImage(b, component))
                .AddTo(component);
        }

        private void SetButtonImage(bool b, InventoryComponent component)
        {
            var nextImage = component.secondarySprite; 
            if (!b)
            {
                nextImage = component.primarySprite;
            }

            component.image.sprite = nextImage;
        }

        private void InitArrows(InventoryComponent component)
        {
            component.arrows.RemoveAllChildren();
            
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            CreateArrowElement(component, currentLevelComponent.maxTopArrows.Value, ForegroundCellType.Top);
            CreateArrowElement(component, currentLevelComponent.maxLeftArrows.Value, ForegroundCellType.Left);
            CreateArrowElement(component, currentLevelComponent.maxRightArrows.Value, ForegroundCellType.Right);
            CreateArrowElement(component, currentLevelComponent.maxBottomArrows.Value, ForegroundCellType.Bottom);
        }

        private void CreateArrowElement(InventoryComponent component, int amount, ForegroundCellType foregroundCellType)
        {
            if (foregroundCellType == ForegroundCellType.Empty || amount == 0) return;

            var arrowElement = Object.Instantiate(component.arrowElementPrefab, component.arrows.transform);
            arrowElement.name = "ArrowElement " + foregroundCellType;
            var arrowElementComponent = arrowElement.GetComponent<ArrowElementComponent>();
            arrowElementComponent.foregroundCellType = foregroundCellType;
            arrowElementComponent.amount.text = amount + " x";
            arrowElementComponent.arrow.sprite =
                component.arrowSprites[(int)foregroundCellType-1];
        }
    }
}