using SystemBase.Core;
using Systems.Grid;
using Systems.UI.Events;
using UniRx;
using UnityEngine;

namespace Systems.UI
{
    [GameSystem]
    public class ArrowElementSystem : GameSystem<ArrowElementComponent>
    {
        public override void Register(ArrowElementComponent component)
        {
            MessageBroker.Default.Receive<UpdateArrowElementMessage>()
                .Subscribe(msg => SetArrowElement(component, msg.amount, msg.image, msg.foregroundCellType))
                .AddTo(component);
        }

        private void SetArrowElement(ArrowElementComponent component, int amount, Sprite image,
            ForegroundCellType foregroundCellType)
        {
            if(component.foregroundCellType != foregroundCellType) { return; }
            component.foregroundCellType = foregroundCellType;
            component.amount.text = amount + " x";
            component.arrow.sprite = image;
        }
    }
}