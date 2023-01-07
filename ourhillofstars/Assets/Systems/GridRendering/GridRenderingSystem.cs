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
    public class GridRenderingSystem : GameSystem<MainGridComponent, BackgroundCellComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.backgroundCells = new GameObject[component.dimensions.x * component.dimensions.y];
            component.foregroundCells = new GameObject[component.dimensions.x * component.dimensions.y];

            component.gridsInitialized
                .Subscribe(InitGridRendering)
                .AddTo(component);

            MessageBroker.Default.Receive<GridUpdateMsg<BackgroundCellType>>()
                .Select(msg => (msg, component))
                .Subscribe(UpdateGrid)
                .AddTo(component);
        }

        private void UpdateGrid((GridUpdateMsg<BackgroundCellType> msg, MainGridComponent component) tuple)
        {
            var cell = tuple.component.backgroundCells[tuple.msg.Index].GetComponent<BackgroundCellComponent>();
            cell.type.Value = tuple.msg.CellType;
        }

        private void InitGridRendering(MainGridComponent grid)
        {
            var backgroundPrefab = IoC.Game.PrefabByName("BackgroundGridCell");
            var foregroundPrefab = IoC.Game.PrefabByName("ForegroundGridCell");

            var backgroundParent = grid.GetComponentInChildren<BackgroundParentComponent>();
            var foregroundParent = grid.GetComponentInChildren<ForegroundParentComponent>();
            
            var max = grid.dimensions.x * grid.dimensions.y;
            for (var i = 0; i < max; i++)
            {
                var x = i % grid.dimensions.x;
                var y = i / grid.dimensions.x;
                grid.backgroundCells[i] = Object.Instantiate(backgroundPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 180, 0),
                    backgroundParent.transform);
                grid.foregroundCells[i] = Object.Instantiate(foregroundPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 180, 0),
                    foregroundParent.transform);
            }
        }

        public override void Register(BackgroundCellComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.type
                .Subscribe(_ => AnimateCellChange(component))
                .AddTo(component);
        }

        private void AnimateCellChange(BackgroundCellComponent backgroundCell)
        {
            if (backgroundCell.type.Value != BackgroundCellType.Harvested)
                Observable.FromMicroCoroutine(() => SwitchGridCell(backgroundCell))
                    .Subscribe()
                    .AddTo(backgroundCell);
            else
                backgroundCell.rendererCache.material.mainTexture = backgroundCell.images[(int)backgroundCell.type.Value];
        }

        private IEnumerator SwitchGridCell(BackgroundCellComponent backgroundCell)
        {
            const float animationTime = 30f;
            for (var i = 0; i < animationTime; i++)
            {
                backgroundCell.transform.RotateAround(backgroundCell.transform.position, Vector3.back, 3);
                yield return null;
            }

            backgroundCell.rendererCache.material.mainTexture = backgroundCell.images[(int)backgroundCell.type.Value];

            for (var i = 0; i < animationTime; i++)
            {
                backgroundCell.transform.RotateAround(backgroundCell.transform.position, Vector3.back, -3);
                yield return null;
            }
        }
    }
}