using System;
using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Grid;
using Systems.GridRendering;
using UniRx;
using UnityEngine;

namespace Systems.GridInteraction
{
    [GameSystem]
    public class GridInteractionSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent grid)
        {
            grid.gridsInitialized
                .WhereNotNull()
                .Subscribe(InitializeInteractions)
                .AddTo(grid);
        }

        private void InitializeInteractions(MainGridComponent grid)
        {
            SystemUpdate(grid)
                .Subscribe(StartInteraction)
                .AddTo(grid);
        }

        private static void StartInteraction(MainGridComponent grid)
        {
            var fGrid = grid.foregroundGrid;
            var fGridComponent = grid.GetComponentInChildren<ForegroundParentComponent>();
            var ray = fGridComponent.mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out var hit) && Input.GetMouseButtonDown(0)) {
                var x = (int)(hit.point.x + 0.5);
                var y = (int)(hit.point.z + 0.5);

                var maxValue = Enum.GetValues(typeof(ForegroundCellType)).Cast<int>().Last() + 1;
                var nextCellType = (int)(fGrid.Cell(x, y)) + 1;
                nextCellType %= maxValue;
                fGrid.Cell(x, y, (ForegroundCellType)nextCellType);
            }
        }
    }
}