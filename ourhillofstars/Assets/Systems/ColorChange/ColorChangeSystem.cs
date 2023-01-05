using SystemBase.Core;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.ColorChange
{
    [GameSystem]
    public class ColorChangeSystem : GameSystem<ColorChangeComponent>
    {
        public override void Register(ColorChangeComponent component)
        {
            component.myRenderer = component.GetComponent<Renderer>();

            component.ColorChangeCommand.Subscribe(b => ChangeColor(component));
        }

        private static void ChangeColor(ColorChangeComponent component)
        {
            component.myRenderer.material.color = component.color;
        }
    }
}