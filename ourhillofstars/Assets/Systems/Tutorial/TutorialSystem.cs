using System;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Drescher;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels;
using Systems.Levels.Events;
using UniRx;

namespace Systems.Tutorial
{
    public enum TutorialStep
    {
        None,
        AddArrow,
        RotateArrow,
        RemoveArrow,
        ClickStart
    }

    [GameSystem]
    public class TutorialSystem : GameSystem<TutorialComponent, MainGridComponent>
    {
        private MainGridComponent _grid;

        public override void Register(TutorialComponent component)
        {
            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Delay(TimeSpan.FromSeconds(2))
                .Subscribe(_ => StartTutorial(component))
                .AddTo(component);

            component.currentStep
                .Subscribe(newStep => ShowObjectsDependingOnStep(newStep, component))
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Subscribe(_ => HideAllTutorialSteps(component))
                .AddTo(component);

            MessageBroker.Default.Receive<AskToGoToNextLevelMsg>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Subscribe(_ => HideAllTutorialSteps(component))
                .AddTo(component);
        }

        private void ShowObjectsDependingOnStep(TutorialStep newStep, TutorialComponent component)
        {
            switch (newStep)
            {
                case TutorialStep.None:
                    component.messageAddArrow.SetActive(false);
                    component.messageClickStart.SetActive(false);
                    component.messageRemoveArrow.SetActive(false);
                    component.messageRotateArrow.SetActive(false);
                    break;
                case TutorialStep.AddArrow:
                    component.messageAddArrow.SetActive(true);
                    break;
                case TutorialStep.RotateArrow:
                    _grid.foregroundGrid.Cell(5, 8, ForegroundCellType.Left);
                    component.messageRotateArrow.SetActive(true);
                    component.messageAddArrow.SetActive(false);
                    break;
                case TutorialStep.RemoveArrow:
                    _grid.foregroundGrid.Cell(5, 3, ForegroundCellType.Left);
                    component.messageRemoveArrow.SetActive(true);
                    component.messageRotateArrow.SetActive(false);
                    break;
                case TutorialStep.ClickStart:
                    component.messageClickStart.SetActive(true);
                    component.messageRemoveArrow.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStep), newStep, null);
            }
        }

        private void StartTutorial(TutorialComponent component)
        {
            if (!_grid) return;
            component.currentStep.Value = TutorialStep.AddArrow;
        }

        private static void HideAllTutorialSteps(TutorialComponent component)
        {
            component.currentStep.Value = TutorialStep.None;
        }

        public override void Register(MainGridComponent component)
        {
            _grid = component;
        }
    }
}