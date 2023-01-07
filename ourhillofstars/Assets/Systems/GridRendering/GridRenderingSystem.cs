using System;
using System.Collections;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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

            MessageBroker.Default.Receive<GridUpdateMsg<BackgroundCellType>>()
                .Select(msg => (msg, component))
                .Subscribe(UpdateGrid)
                .AddTo(component);

            Observable.Interval(TimeSpan.FromSeconds(2))
                .Subscribe(_ => component.backgroundGrid.Cell(
                    Random.Range(0, 12), 
                    Random.Range(0, 12), 
                    (BackgroundCellType)Random.Range(0, (int)(BackgroundCellType.Harvested) + 1)));
        }

        private void UpdateGrid((GridUpdateMsg<BackgroundCellType> msg, MainGridComponent component) tuple)
        {
            var cell = tuple.component.backgroundCells[tuple.msg.Index].GetComponent<CellComponent>();
            cell.type.Value = tuple.msg.CellType;
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
            }
        }

        public override void Register(CellComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.type
                .Subscribe(_ => AnimateCellChange(component))
                .AddTo(component);
        }

        private void AnimateCellChange(CellComponent cell)
        {
            if (cell.type.Value != BackgroundCellType.Harvested)
                Observable.FromMicroCoroutine(() => SwitchGridCell(cell))
                    .Subscribe()
                    .AddTo(cell);
            else
                cell.rendererCache.material.mainTexture = cell.images[(int)cell.type.Value];
        }

        private IEnumerator SwitchGridCell(CellComponent cell)
        {
            const float animationTime = 30f;
            for (var i = 0; i < animationTime; i++)
            {
                cell.transform.RotateAround(cell.transform.position, Vector3.back, 3);
                yield return null;
            }

            cell.rendererCache.material.mainTexture = cell.images[(int)cell.type.Value];

            for (var i = 0; i < animationTime; i++)
            {
                cell.transform.RotateAround(cell.transform.position, Vector3.back, -3);
                yield return null;
            }
        }
    }
}