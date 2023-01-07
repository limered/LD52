using SystemBase.Core;
using Systems.Grid;
using Systems.GridRendering;
using UniRx;
using UnityEngine;

namespace Systems.GridInteraction
{
    [GameSystem]
    public class GridInteractionSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.gridsInitialized
                .Subscribe(InitializeInteractions)
                .AddTo(component);
        }

        private void InitializeInteractions(MainGridComponent grid)
        {
            var fGrid = grid.foregroundGrid;
            var fGridComponent = grid.GetComponentInChildren<ForegroundParentComponent>();

            var ray = fGridComponent.mainCamera.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out var hit)) {
                var objectHit = hit.transform.position;
                
                Debug.Log(objectHit.x);
            }
        }
    }
}