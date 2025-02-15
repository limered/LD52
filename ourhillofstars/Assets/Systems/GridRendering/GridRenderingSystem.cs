﻿using System.Collections;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using UniRx;
using UnityEngine;

namespace Systems.GridRendering
{
    [GameSystem]
    public class GridRenderingSystem : GameSystem<MainGridComponent, BackgroundCellComponent, ForegroundCellComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.backgroundCells = new GameObject[component.dimensions.x * component.dimensions.y];
            component.foregroundCells = new GameObject[component.dimensions.x * component.dimensions.y];

            component.gridsInitialized
                .WhereNotNull()
                .Subscribe(InitGridRendering)
                .AddTo(component);

            MessageBroker.Default.Receive<GridUpdateMsg<BackgroundCellType>>()
                .Select(msg => (msg, component))
                .Subscribe(UpdateBackgroundGrid)
                .AddTo(component);

            MessageBroker.Default.Receive<GridUpdateMsg<ForegroundCellType>>()
                .Select(msg => (msg, component))
                .Subscribe(UpdateForegroundGrid)
                .AddTo(component);
        }

        private void UpdateBackgroundGrid((GridUpdateMsg<BackgroundCellType> msg, MainGridComponent component) tuple)
        {
            var cell = tuple.component.backgroundCells[tuple.msg.Index].GetComponent<BackgroundCellComponent>();
            cell.type.Value = tuple.msg.CellType;
        }

        private void UpdateForegroundGrid((GridUpdateMsg<ForegroundCellType> msg, MainGridComponent component) tuple)
        {
            var cell = tuple.component.foregroundCells[tuple.msg.Index].GetComponent<ForegroundCellComponent>();
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
                grid.backgroundCells[i] = Object.Instantiate(backgroundPrefab, new Vector3(x, 0, y),
                    Quaternion.Euler(0, 180, 0),
                    backgroundParent.transform);
                grid.foregroundCells[i] = Object.Instantiate(foregroundPrefab, new Vector3(x, 1, y),
                    Quaternion.Euler(0, 180, 0),
                    foregroundParent.transform);
            }
        }

        public override void Register(BackgroundCellComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.type
                .Subscribe(_ => AnimateBackgroundCellChange(component))
                .AddTo(component);
        }

        private void AnimateBackgroundCellChange(BackgroundCellComponent cell)
        {
            if (cell.type.Value != BackgroundCellType.Harvested)
                Observable.FromCoroutine(() => SwitchBackgroundGridCell(cell.gameObject,
                        cell.rendererCache, cell.images[(int)cell.type.Value]))
                    .Subscribe()
                    .AddTo(cell);
            else
                cell.rendererCache.material.mainTexture =
                    cell.images[(int)cell.type.Value];
        }

        private static IEnumerator SwitchBackgroundGridCell(GameObject backgroundCell, Renderer renderer, Texture image)
        {
            const float animationTime = 30f;
            for (var i = 0; i < animationTime; i++)
            {
                backgroundCell.transform.RotateAround(backgroundCell.transform.position, Vector3.back, 3);
                yield return new WaitForSeconds(0.01f);
            }
            renderer.material.mainTexture = image;
            for (var i = 0; i < animationTime; i++)
            {
                backgroundCell.transform.RotateAround(backgroundCell.transform.position, Vector3.back, -3);
                yield return new WaitForSeconds(0.01f);
            }
        }

        public override void Register(ForegroundCellComponent component)
        {
            component.rendererCache = component.GetComponent<Renderer>();
            component.type
                .Subscribe(_ => AnimateForegroundCellChange(component))
                .AddTo(component);
        }

        private static void AnimateForegroundCellChange(ForegroundCellComponent cell)
        {
            var mat = cell.rendererCache.material;
            mat.mainTexture = cell.images[(int)cell.type.Value];
            
            var col = mat.color;
            col.a = cell.type.Value == ForegroundCellType.Empty ? 0f : 1f;
            mat.color = col;
        }
    }
}