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
        // RemoveArrow,
        ClickStart
    }

    [GameSystem(typeof(GridSystem))]
    public class TutorialSystem : GameSystem<TutorialComponent, MainGridComponent>
    {
        private MainGridComponent _grid;

        public override void Register(TutorialComponent component)
        {
            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Delay(TimeSpan.FromSeconds(component.waitTime))
                .Subscribe(_ => StartTutorial(component))
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Subscribe(_ => HideAllTutorialSteps(component))
                .AddTo(component);

            MessageBroker.Default.Receive<AskToGoToNextLevelMsg>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Subscribe(_ => HideAllTutorialSteps(component))
                .AddTo(component);

            MessageBroker.Default.Receive<TutorialMessage>()
                .Where(_ => IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex == 0)
                .Subscribe(msg => NoToNextTutorialStep(msg, component))
                .AddTo(component);
        }

        private void NoToNextTutorialStep(TutorialMessage tutorialMessage, TutorialComponent component)
        {
            if(tutorialMessage.stepToEnd != component.currentStep.Value)return;
            switch (tutorialMessage.stepToEnd)
            {
                case TutorialStep.None:
                    break;
                case TutorialStep.AddArrow:
                    _grid.foregroundGrid.Cell(5, 8, ForegroundCellType.Right);
                    component.messageRotateArrow.SetActive(true);
                    component.messageAddArrow.SetActive(false);
                    component.currentStep.Value = TutorialStep.RotateArrow;
                    break;
                case TutorialStep.RotateArrow:
                    _grid.foregroundGrid.Cell(5, 3, ForegroundCellType.Left);
                    component.messageClickStart.SetActive(true);
                    component.messageRotateArrow.SetActive(false);
                    component.currentStep.Value = TutorialStep.ClickStart;
                    break;
                // case TutorialStep.RemoveArrow:
                //     component.messageClickStart.SetActive(true);
                //     component.messageRemoveArrow.SetActive(false);
                //     component.currentStep.Value = TutorialStep.ClickStart;
                //     break;
                case TutorialStep.ClickStart:
                    component.messageAddArrow.SetActive(false);
                    component.messageClickStart.SetActive(false);
                    // component.messageRemoveArrow.SetActive(false);
                    component.messageRotateArrow.SetActive(false);
                    component.currentStep.Value = TutorialStep.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartTutorial(TutorialComponent component)
        {
            if (!_grid) return;
            component.currentStep.Value = TutorialStep.AddArrow;
            component.messageAddArrow.SetActive(true);
        }

        private static void HideAllTutorialSteps(TutorialComponent component)
        {
            component.currentStep.Value = TutorialStep.None;
            component.messageAddArrow.SetActive(false);
            component.messageClickStart.SetActive(false);
            // component.messageRemoveArrow.SetActive(false);
            component.messageRotateArrow.SetActive(false);
        }

        public override void Register(MainGridComponent component)
        {
            _grid = component;
        }
    }
}