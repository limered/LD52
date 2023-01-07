using SystemBase.Core;
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

            //TODO listen to message to update arrows
            // MessageBroker.Default
            // .Publish(
            //     new UpdateArrowElementMessage
            //     {
            //         amount = amount,
            //         image = component.arrowSprites[spriteIndex]
            //     }
            // );
        }

        private void InitArrows(InventoryComponent component)
        {
            //TODO load arrows from somewhere
            CreateArrowElement(component, 1, 0);
            CreateArrowElement(component, 4, 1);
            CreateArrowElement(component, 3, 2);
            CreateArrowElement(component, 2, 3);
        }

        private void CreateArrowElement(InventoryComponent component, int amount, int spriteIndex)
        {
            var arrowElement = Object.Instantiate(component.arrowElementPrefab, component.arrows.transform);
            arrowElement.name = "ArrowElement " + spriteIndex;
            arrowElement.GetComponent<ArrowElementComponent>().amount.text = amount + " x";
            arrowElement.GetComponent<ArrowElementComponent>().arrow.sprite = component.arrowSprites[spriteIndex];
        }
    }
}