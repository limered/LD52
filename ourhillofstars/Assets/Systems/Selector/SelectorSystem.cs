using SystemBase.Core;
using Systems.GridInteraction;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Systems.Selector
{
    [GameSystem(typeof(GridInteractionSystem))]
    public class SelectorSystem : GameSystem<SelectorComponent>
    {
        public override void Register(SelectorComponent component)
        {
            component.myRenderer = component.GetComponent<Renderer>();
            component.availableSprite = component.myRenderer.material.mainTexture;
            component.shouldBeInvisible
                .Subscribe(b =>SetSelectorInvisibility(b, component))
                .AddTo(component);
            SystemUpdate(component).
                Subscribe(UpdatePos)
                .AddTo(component);
            component.shouldChangeTexture
                .Subscribe(b => ChangeTexture(b, component))
                .AddTo(component);
        }
        
        private static void SetSelectorInvisibility(bool visibility, SelectorComponent component)
        {
            component.myRenderer.enabled = visibility;
        }
        private static void UpdatePos(SelectorComponent component)
        {
            component.transform.position = new Vector3(component.targetCoord.x,1f,component.targetCoord.y);
        }

        private static void ChangeTexture(bool shouldChangeTexture,SelectorComponent component)
        {
            component.myRenderer.material.mainTexture = shouldChangeTexture ? component.notAvailableSprite : component.availableSprite;
        }
    }
}