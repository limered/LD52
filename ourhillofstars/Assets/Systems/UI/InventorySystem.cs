using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using Systems.Levels;
using UnityEngine;

namespace Systems.UI
{
    [GameSystem]
    public class InventorySystem : GameSystem<InventoryComponent>
    {
        public override void Register(InventoryComponent component)
        {
            InitArrows(component);
        }

        private void InitArrows(InventoryComponent component)
        {
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();

            CreateArrowElement(component, currentLevelComponent.topArrows.Value, ForegroundCellType.Top);
            CreateArrowElement(component, currentLevelComponent.leftArrows.Value, ForegroundCellType.Left);
            CreateArrowElement(component, currentLevelComponent.rightArrows.Value, ForegroundCellType.Right);
            CreateArrowElement(component, currentLevelComponent.bottomArrows.Value, ForegroundCellType.Bottom);
        }

        private void CreateArrowElement(InventoryComponent component, int amount, ForegroundCellType foregroundCellType)
        {
            if (foregroundCellType == ForegroundCellType.Empty) return;

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