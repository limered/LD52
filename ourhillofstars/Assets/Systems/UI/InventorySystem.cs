using SystemBase.Core;
using Systems.Grid;
using Systems.GridInteraction.Events;
using Systems.UI.Events;
using UniRx;
using UnityEngine;

namespace Systems.UI
{
    [GameSystem]
    public class InventorySystem : GameSystem<InventoryComponent>
    {
        public override void Register(InventoryComponent component)
        {
            InitArrows(component);
            
            MessageBroker.Default.Receive<SetForegroundCellTypeMessage>()
                .Subscribe(msg =>
                {
                    if (msg.foregroundCellType == ForegroundCellType.Empty) return;
                    
                    MessageBroker.Default
                        .Publish(
                            new UpdateArrowElementMessage
                            {
                                foregroundCellType = msg.foregroundCellType,
                                amount = 1, //TODO calculate amount
                                image = component.arrowSprites[(int)msg.foregroundCellType - 1]
                            }
                        );
                })
                .AddTo(component);
        }

        private void InitArrows(InventoryComponent component)
        {
            //TODO load arrows from somewhere
            CreateArrowElement(component, 4, ForegroundCellType.Top);
            CreateArrowElement(component, 4, ForegroundCellType.Left);
            CreateArrowElement(component, 3, ForegroundCellType.Right);
            CreateArrowElement(component, 2, ForegroundCellType.Bottom);
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