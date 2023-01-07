using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.GridRendering
{
    [GameSystem]
    public class GridRenderingSystem : GameSystem<MainGridComponent, CellComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.backgroundCells = new GameObject[component.dimensions.x * component.dimensions.y];

            component.gridsInitialized
                .Subscribe(InitGridRendering)
                .AddTo(component);

            MessageBroker.Default.Receive<GridUpdateMsg>()
                .Select(msg => (msg, component))
                .Subscribe(UpdateGrid)
                .AddTo(component);
        }

        private void UpdateGrid((GridUpdateMsg msg, MainGridComponent component) tuple)
        {
            tuple.component.backgroundCells[tuple.msg.Index]
                .GetComponent<CellComponent>().type.Value = tuple.msg.CellType;
        }

        private void InitGridRendering(MainGridComponent grid)
        {
            var prefab = IoC.Game.PrefabByName("BackgroundGridCell");
            var max = grid.dimensions.x * grid.dimensions.y;
            for (var i = 0; i < max; i++)
            {
                var x = i % grid.dimensions.x;
                var y = i / grid.dimensions.x;
                grid.backgroundCells[i] = Object.Instantiate(prefab, new Vector3(x, 0, y), Quaternion.Euler(0, 180, 0),
                    grid.transform);
                
                // TODO remove after test
                if (i % 2 == 0) grid.backgroundCells[i].GetComponent<CellComponent>().type.Value = GridCellType.Start;
            }
        }

        public override void Register(CellComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.rendererCache.material.mainTexture = component.images[(int)component.type.Value];

            component.type.Subscribe(type =>
                component.rendererCache.material.mainTexture = component.images[(int)type]);
        }
    }
}