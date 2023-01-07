using System;
using SystemBase.Core;
using SystemBase.Utils;
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
            switch (component.foregroundCellType)
            {
                case ForegroundCellType.Empty:
                    break;
                case ForegroundCellType.Left:
                    currentLevelComponent.topArrows.Subscribe(value =>
                        SetArrowElement(component, value, ForegroundCellType.Left));
                    break;
                case ForegroundCellType.Top:
                    currentLevelComponent.leftArrows.Subscribe(value =>
                        SetArrowElement(component, value, ForegroundCellType.Top));
                    break;
                case ForegroundCellType.Right:
                    currentLevelComponent.rightArrows.Subscribe(value =>
                        SetArrowElement(component, value, ForegroundCellType.Right));
                    break;
                case ForegroundCellType.Bottom:
                    currentLevelComponent.bottomArrows.Subscribe(value =>
                        SetArrowElement(component, value, ForegroundCellType.Bottom));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetArrowElement(ArrowElementComponent component, int amount,
            ForegroundCellType foregroundCellType)
        {
            component.foregroundCellType = foregroundCellType;
            component.amount.text = amount + " x";
        }
    }
}