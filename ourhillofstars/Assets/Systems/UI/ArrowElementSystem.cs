using System;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels;
using UniRx;

namespace Systems.UI
{
    [GameSystem]
    public class ArrowElementSystem : GameSystem<ArrowElementComponent>
    {
        public override void Register(ArrowElementComponent component)
        {
            var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
            
            currentLevelComponent.arrowsUsed.Subscribe(value =>
                SetArrowElement(component, value, ForegroundCellType.Top));
        }

        private void SetArrowElement(ArrowElementComponent component, int amount,
            ForegroundCellType foregroundCellType)
        {
            component.foregroundCellType = foregroundCellType;
            component.amount.text = amount + " x";
        }
    }
}