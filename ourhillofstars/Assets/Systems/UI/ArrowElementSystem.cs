using SystemBase.Core;
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
                .Subscribe(msg => SetArrowElement(component, msg.amount, msg.image))
                .AddTo(component);
        }

        private void SetArrowElement(ArrowElementComponent component, int amount, Sprite image)
        {
            Debug.Log("Set amount to " + amount);
            component.amount.text = amount + " x";
            component.arrow.sprite = image;
        }
    }
}